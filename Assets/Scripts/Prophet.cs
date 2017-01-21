using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prophet : Node {

	[SerializeField]
	private Node currentNodePlacement;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public bool CanMoveToNode(Node node) {
		// Validate this prophet can move the distance to the given node
		return false;
	}

	public void MoveProphet(Node newNode) {
		// Move the prophet to the new node location
		// StartCorouting(DoMoveProphet);
	}

	private IEnumerator DoMoveProphet(Vector2 newLocation) {
		return null;
	}
}
