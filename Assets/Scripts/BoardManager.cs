using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    private const string DEFAULT_BOARD_FILE = "board";

    public GameObject nodePrefab;

    // Use this for initialization
    void Awake()
	{
		nodes = new Dictionary<Vector2, Node>();
        MakeNodes();
        //StartCoroutine(Test ()); //!!
    }

    public Node GetNodeAt(Vector2 gridPosition)
    {
        return nodes[gridPosition];
    }

    private IEnumerator Test()
    {
        foreach (var nodePair in nodes)
        {
            var node = nodePair.Value;
            node.TestNode(true);
            yield return new WaitForSeconds(1f);
            node.TestNode(false);
        }
    }

    /// <summary>
    /// All the nodes in the game. 
    /// Vector2 is the Grid Position of a specific Node for easy lookup.
    /// </summary>
    //[SerializeField]
    public Dictionary<Vector2, Node> nodes { get; set; }

    private void MakeNodes()
    {
        IList<Vector2> playerStarts;
        var boardArray = GetBoardArray(DEFAULT_BOARD_FILE, out playerStarts);
        GameManager.Instance.PlayerStarts = playerStarts;

        // create nodes from array
        const float halfRoot3 = 0.8660254038f;
        var rows = boardArray.Length;
        var columns = boardArray[0].Length;
        for (var i = rows - 1; i >= 0; i--)
        {
            for (var j = 0; j < columns; j++)
            {
                if (!boardArray[i][j]) continue;
                var gridPosition = BoardArrayToGridPosition(i, j, rows);
                var gridX = gridPosition.x;
                var gridY = gridPosition.y;

                // create the node at the proper place in the world
                var worldPosition = new Vector2(0.75f * gridY, halfRoot3 * (0.5f * (gridY % 2) + gridX)); 
                var node = Instantiate(nodePrefab, worldPosition, Quaternion.identity, transform);

                // track the node and keep its grid position
                var nodeScript = node.GetComponent<Node>();
                nodeScript.GridPosition = gridPosition;
                nodes[gridPosition] = nodeScript;
            }
        }

        // connect nodes as edges
        ComputeNeighbors();
    }

    private void ComputeNeighbors()
    {
        foreach (var node in nodes.Values)
        {
            node.ComputeNeighbors();
        }
    }

    private bool[][] GetBoardArray(string fileName, out IList<Vector2> playerStarts)
    {
        var playerStartsByLetter = new Dictionary<char, Vector2>();
        // read file into string
        var file = Resources.Load<TextAsset>(fileName);
        var boardString = file.text;

        // convert string to array
        var lines = boardString.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        var rows = lines.Length;
        var columns = lines[0].Length;
        bool[][] boardArray = new bool[rows][];
        for (var i = 0; i < rows; i++)
        {
            boardArray[i] = new bool[columns];
            for (var j = 0; j < columns; j++)
            {
                // track final grid positions of 
                if (char.IsLetter(lines[i][j]))
                {
                    playerStartsByLetter[lines[i][j]] = BoardArrayToGridPosition(i, j, rows);
                }
                boardArray[i][j] = lines[i][j] != '0';
            }
        }

        var playerStartLetters = new List<char>(playerStartsByLetter.Keys);
        playerStartLetters.Sort();
        playerStarts = new List<Vector2>(playerStartLetters.Count);
        foreach (var playerStartLetter in playerStartLetters)
        {
            playerStarts.Add(playerStartsByLetter[playerStartLetter]);
        }
        return boardArray;
    }

    private Vector2 BoardArrayToGridPosition(int currentRow, int currentColumn, int totalRows)
    {
        var gridX = currentColumn;
        var gridY = totalRows - 1 - currentRow;
        return new Vector2(gridX, gridY);
    }
}
