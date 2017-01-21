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
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	private IEnumerator Test()
	{
		foreach (var nodePair in nodes)
		{
			var node = nodePair.Value;
			var nodeSpriteRenderer = node.gameObject.GetComponent<SpriteRenderer> ();
			nodeSpriteRenderer.color = Color.blue;
			foreach (var connection in node.edges) {
				var edge = connection.Value;
				var otherSpriteRenderer = edge.Follow(connection.Key).gameObject.GetComponent<SpriteRenderer> ();
				otherSpriteRenderer.color = Color.red;
			}
			yield return new WaitForSeconds (0.5f);
			nodeSpriteRenderer.color = Color.white;
			foreach (var connection in node.edges) {
				var otherNode = connection.Value;
				var otherSpriteRenderer = otherNode.Follow(connection.Key).gameObject.GetComponent<SpriteRenderer> ();
				otherSpriteRenderer.color = Color.white;
			}
		}
	}

	/// <summary>
	/// All the nodes in the game. 
	/// Vector2 is the Grid Position of a specific Node for easy lookup.
	/// </summary>
	//[SerializeField]
	private Dictionary<Vector2, Node> nodes = new Dictionary<Vector2, Node> ();

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
				var gridX = j;
				var gridY = rows - 1 - i;

				// create the node at the proper place in the world
				var worldPosition = new Vector2(halfRoot3 * (0.5f * (gridY % 2) + gridX), 0.75f * gridY);
				var node = Instantiate(nodePrefab, worldPosition, Quaternion.identity, transform);

				// track the node and keep its grid position
				var nodeScript = node.GetComponent<Node>();
				var gridPosition = new Vector2 (gridX, gridY);
				nodeScript.GridPosition = gridPosition;
				nodes[gridPosition] = nodeScript;
			}
		}

		// connect nodes as edges
		ComputeNeighbors();
	}

	private void ComputeNeighbors ()
	{
		foreach (var node in nodes.Values)
		{
			node.ComputeNeighbors ();
		}
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
