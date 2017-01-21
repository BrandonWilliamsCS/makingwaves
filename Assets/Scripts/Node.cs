using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour {

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

	private Vector2 _gridPosition;
	public Vector2 GridPosition {
		get {
			return _gridPosition;
		}
		set {
			_gridPosition = value;
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

	// Use this for initialization
	void Start () {
		EmitInfluence ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void EmitInfluence() {
		if (InfluenceEmitter != null) {
		// TODO fire particle effect
			InfluenceEmitter.Play();
		}
	}

	#region Model
	public int boardX;
	public int boardY;

	public override bool Equals (object obj)
	{
		var otherNode = obj as Node;
		if (otherNode == null)
		{
			return false;
		}
		
		return boardX == otherNode.boardX && boardY == otherNode.boardY; 
	}
	
	public override int GetHashCode()
	{
		return boardX * 10000 + boardY;
	}
	#endregion

	private class Edge
	{
		public Node Left { get; set; }
		public Node Right { get; set; }
	}
}
