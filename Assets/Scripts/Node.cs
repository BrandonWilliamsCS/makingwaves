using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour {

	#region Vectors to neighbors
	public static readonly float ROOT_3 = Mathf.Sqrt(3);
	public static readonly Vector2 TO_1_OCLOCK = new Vector2(ROOT_3 / 4, 0.75f);
	public static readonly Vector2 TO_3_OCLOCK = new Vector2(ROOT_3 / 2, 0);
	public static readonly Vector2 TO_5_OCLOCK = Vector2.Scale(TO_1_OCLOCK, new Vector2(1, -1));
	public static readonly Vector2 TO_7_OCLOCK = Vector2.Scale(TO_1_OCLOCK, new Vector2(-1, -1));
	public static readonly Vector2 TO_9_OCLOCK = -1 * new Vector2(1, ROOT_3);
	public static readonly Vector2 TO_11_OCLOCK = Vector2.Scale(TO_1_OCLOCK, new Vector2(-1, 1));

	public static readonly IDictionary<int, Vector2> DIRECTIONS = new Dictionary<int, Vector2>()
	{
		{ 1, TO_1_OCLOCK },
		{ 3, TO_3_OCLOCK },
		{ 5, TO_5_OCLOCK },
		{ 7, TO_7_OCLOCK },
		{ 9, TO_9_OCLOCK },
		{ 11, TO_11_OCLOCK }
	};
	#endregion

	[SerializeField]
	private Player Owner;

	[SerializeField]
	private int _topHealth = 10;
	public int TopHealth {
		get {
			return _topHealth;
		}
	}

	[SerializeField]
	private int _healthThreshold = 7;
	/// <summary>
	/// Gets the health threshold.
	/// This is how many influence points it takes before we can emit influence
	/// </summary>
	/// <value>The health threshold.</value>
	public int HealthThreshold {
		get {
			return _healthThreshold;
		}
	}

	[SerializeField]
	private int _currentHealth = 0;
	/// <summary>
	/// Gets the current health.
	/// Current health 0 denotes a neutral node
	/// </summary>
	/// <value>The current health.</value>
	public int CurrentHealth {
		get {
			return _currentHealth;
		}
	}

	[SerializeField]
	private int _influenceWeight = 1;
	public int InfluenceWeight {
		get {
			return _influenceWeight;
		}
	}

	private ParticleSystem _influenceEmitter;
	private ParticleSystem InfluenceEmitter {
		get {
			if (_influenceEmitter == null) {
				_influenceEmitter = GetComponentInChildren<ParticleSystem> ();
			}
			return _influenceEmitter;
		}
	}

	private SpriteRenderer _spriteRenderer;
	protected SpriteRenderer MySpriteRenderer {
		get {
			if (_spriteRenderer == null) {
				_spriteRenderer = this.GetComponent<SpriteRenderer> ();
			}
			return _spriteRenderer;
		}
	}

	// Use this for initialization
	void Start () {
		//EmitInfluence ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ComputeNeighbors()
	{
		var collider = GetComponent<PolygonCollider2D> ();
		foreach (var clockDirection in DIRECTIONS)
		{
			// find adjacent nodes, but only going left to right
			if (clockDirection.Key >= 6) continue;
			collider.enabled = false;
			var hit = Physics2D.Raycast (transform.position, clockDirection.Value, 1f);
			collider.enabled = true;
			if (hit.transform == null) continue;

			// create an edge from this to that and connect each object together
			var otherNode = hit.transform.gameObject.GetComponent<Node>();
			var edge = new Edge (this, otherNode, clockDirection.Value);
			edges [clockDirection.Value] = edge;
			var oppositeDirection = DIRECTIONS [(clockDirection.Key + 6) % 12];
			otherNode.edges [oppositeDirection] = edge;
		}
	}

	public void EmitInfluence() {
		if (InfluenceEmitter != null) {
			if (MySpriteRenderer != null) {
				var main = InfluenceEmitter.main;
				main.startColor = MySpriteRenderer.color;
			}
			InfluenceEmitter.Play();
		}
		//TODO: Evangelize neighbors
	}

	#region Model
	public Vector2 GridPosition { get; set;	}

	private IDictionary<Vector2, Edge> edges = new Dictionary<Vector2, Edge>();

	public override bool Equals (object obj)
	{
		var otherNode = obj as Node;
		if (otherNode == null)
		{
			return false;
		}
		
		return GridPosition.Equals(otherNode.GridPosition); 
	}
	
	public override int GetHashCode()
	{
		return GridPosition.GetHashCode();
	}

	private class Edge
	{
		public Node Left { get; set; }
		public Node Right { get; set; }
		public Vector2 LeftToRight { get; set; }

		public Edge(Node left, Node right, Vector2 leftToRight)
		{
			Left = left;
			Right = right;
			LeftToRight = leftToRight;
		}

		public Node Follow(Vector2 direction)
		{
			if (direction == LeftToRight) {
				return Right;
			} else {
				return Left;
			}
		}
	}
	#endregion


	public void TestNode(bool color)
	{
		var nodeSpriteRenderer = GetComponent<SpriteRenderer> ();
		nodeSpriteRenderer.color = color ? Color.blue : Color.white;
		foreach (var connection in edges) {
			var edge = connection.Value;
			var otherSpriteRenderer = edge.Follow(connection.Key).gameObject.GetComponent<SpriteRenderer> ();
			otherSpriteRenderer.color = color ? Color.red : Color.white;
		}
	}
}
