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

    // TODO: richer board format for more starting configurations
    public IList<Vector2> PlayerStarts { get; set; }

    /// <summary>
    /// All the nodes on the board. 
    /// Vector2 is the Grid Position of a specific Node for easy lookup.
    /// </summary>
    public IDictionary<Vector2, Node> Nodes { get; set; }

    #region Game Loop/Events
    void Awake()
    {
        Nodes = new Dictionary<Vector2, Node>();
        MakeNodes(DEFAULT_BOARD_FILE);
        //StartCoroutine(Test ()); // TODO: better to have specialized test/debug options
    }
    #endregion

    private void MakeNodes(string boardFileName) // TODO: split functions a little nicer
    {
        var boardArray = GetBoardArray(boardFileName);

        // create nodes from array
        var rows = boardArray[0].Length;
        var columns = boardArray.Length;
        for (var i = 0; i < columns; i++)
        {
            for (var j = 0; j < rows; j++)
            {
                if (!boardArray[i][j]) continue;

                // create the node at the proper place in the world
                var worldPosition = WorldPositionForHexGridPosition(i, j);
                var node = Instantiate(nodePrefab, worldPosition, Quaternion.identity, transform);

                // track the node and keep its grid position
                var nodeScript = node.GetComponent<Node>();
                var gridPosition = new Vector2(i, j);
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
    
    private bool[][] GetBoardArray(string fileName)
    {
        var playerStartsByLetter = new Dictionary<char, Vector2>();
        // read file into string
        var file = Resources.Load<TextAsset>(fileName);
        var boardString = file.text;

        // convert string to array
        var lines = boardString.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        var rows = lines.Length;
        var columns = lines[0].Length;
        bool[][] boardArray = new bool[columns][];
        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < columns; j++)
            {
                if (i == 0) boardArray[j] = new bool[rows];
                var gridX = j;
                var gridY = rows - 1 - i;
                // track final grid positions of player locations as well.
                if (char.IsLetter(lines[i][j]))
                {
                    playerStartsByLetter[lines[i][j]] = new Vector2(gridX, gridY);
                }
                boardArray[gridX][gridY] = lines[i][j] != '0';
            }
        }

        var playerStartLetters = new List<char>(playerStartsByLetter.Keys);
        playerStartLetters.Sort();
        PlayerStarts = new List<Vector2>(playerStartLetters.Count);
        foreach (var playerStartLetter in playerStartLetters)
        {
            PlayerStarts.Add(playerStartsByLetter[playerStartLetter]);
        }
        return boardArray;
    }

    private Vector2 WorldPositionForHexGridPosition(int x, int y)
    {
        const float halfRoot3 = 0.8660254038f;
        return new Vector2(0.75f * y, halfRoot3 * (0.5f * (y % 2) + x));
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
