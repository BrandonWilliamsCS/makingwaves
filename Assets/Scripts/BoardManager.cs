using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    // TODO: configurable, in editor or game settings
    private const string DEFAULT_BOARD_FILE = "board";

    #region Unity Injected
    public GameObject nodePrefab;
    #endregion

    /// <summary>
    /// All the nodes in the game. 
    /// Vector2 is the Grid Position of a specific Node for easy lookup.
    /// </summary>
    public IDictionary<Vector2, Node> Nodes { get; set; } //!! TODO: expose read-only dictionary, adding special modders as necessary.

    #region Game Loop/Events
    // Use this for initialization
    void Awake()
    {
        Nodes = new Dictionary<Vector2, Node>();
        MakeNodes();
        //StartCoroutine(Test ()); //!! TODO: better to have specialized test/debug options
    }
    #endregion

    //!! TODO: replace with direct look at Nodes.
    public Node GetNodeAt(Vector2 gridPosition)
    {
        return Nodes[gridPosition];
    }

    private void MakeNodes() //!! TODO: accept board file(name), split functions a little nicer
    {
        IList<Vector2> playerStarts;
        var boardArray = GetBoardArray(DEFAULT_BOARD_FILE, out playerStarts);
        GameManager.Instance.PlayerStarts = playerStarts; // TODO: richer board format for more starting configurations

        // create nodes from array
        const float halfRoot3 = 0.8660254038f; //!! TODO: factor raw math out?
        var rows = boardArray.Length;
        var columns = boardArray[0].Length;
        for (var i = rows - 1; i >= 0; i--) //!! TODO: clarify and comment relationships between world/grid, i/x, etc.
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
                Nodes[gridPosition] = nodeScript;
            }
        }

        // connect nodes as edges
        ComputeNeighbors();
    }

    private void ComputeNeighbors()
    {
        foreach (var node in Nodes.Values)
        {
            node.ComputeNeighbors();
        }
    }

    //!! TODO: let's keep the irregular row/col counting within this function; return s.t. board[x][y] is at grid pos. (x, y)
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

    private IEnumerator Test()
    {
        foreach (var nodePair in Nodes)
        {
            var node = nodePair.Value;
            node.TestNode(true);
            yield return new WaitForSeconds(1f);
            node.TestNode(false);
        }
    }
}
