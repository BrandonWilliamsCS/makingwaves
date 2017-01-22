using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prophet : Node {

	private Node currentNode;
	public Node CurrentNode { 
		get { return currentNode; }
		set { 
			currentNode = value;
			transform.position = value.transform.position;
		}
	}

	public Color Color { get { return MySpriteRenderer.color; } set { MySpriteRenderer.color = new Color(value.r, value.g, value.b, 0.3f); } }

	// Update is called once per frame
	void Update () {
		
	}

	public bool CanMoveToNode(Node node) {
		// Validate this prophet can move the distance to the given node
		return currentNode.IsNeighbor(node);
	}

	public bool MoveProphet(Node newNode) {
        // Move the prophet to the new node location
        if (CanMoveToNode(newNode))
        {
            StartCoroutine(DoMoveProphet(newNode.transform.position));
			return true;
        }
		return false;
	}

	private IEnumerator DoMoveProphet(Vector3 newLocation) {
		float speed = 2f;
		while (transform.position != newLocation) {
			float step = speed * Time.deltaTime;
			transform.position = Vector3.MoveTowards (transform.position, newLocation, step);
			yield return null;
		}
	}
}
