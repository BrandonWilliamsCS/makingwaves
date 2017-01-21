using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
	private const string DEFAULT_BOARD_FILE = "board";

	public GameObject nodePrefab;

	// Use this for initialization
	void Start ()
	{
		MakeNodes();
		// TODO: generate board
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	private void MakeNodes ()
	{
		var boardArray = GetBoardArray(DEFAULT_BOARD_FILE);

		// create nodes from array
		const float halfRoot3 = 0.8660254038f;
		var rows = boardArray.Length;
		var columns = boardArray[0].Length;
		for (var i = rows - 1; i >= 0; i--)
		{
			for (var j = 0; j < columns; j++)
			{
				if (!boardArray[i][j]) continue;
				var worldI = rows - 1 - i;
				var worldPosition = new Vector2(halfRoot3 * (0.5f * (worldI % 2) + j), 0.75f * worldI);
				var node = Instantiate(nodePrefab, worldPosition, Quaternion.identity, transform);
				var nodeScript = node.GetComponent<Node>();
			}
		}

		// connect nodes as edges
	}

	private bool[][] GetBoardArray(string fileName)
	{
		// read file into string
		var file = Resources.Load<TextAsset>(fileName);
		var boardString = file.text;

		// convert string to array
		// TODO: include players somehow
		var lines = boardString.Split(new char[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
		var rows = lines.Length;
		var columns = lines[0].Length;
		bool[][] boardArray = new bool[rows][];
		for (var i = 0; i < rows; i++)
		{
			boardArray[i] = new bool[columns];
			for (var j = 0; j < columns; j++)
			{
				boardArray[i][j] = lines[i][j] != '0';
			}
		}

		return boardArray;
	}
}
