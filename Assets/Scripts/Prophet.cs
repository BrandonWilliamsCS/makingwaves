using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prophet : Node
{

    private Node currentNode;
    public Node CurrentNode
    {
        get { return currentNode; }
        set
        {
            if (currentNode != null)
            {
                currentNode.prophets.Remove(this);
            }
            currentNode = value;
            currentNode.prophets.Add(this);
        }
    }

	protected override void Awake()
	{
		base.Awake ();
	}

	private SpriteRenderer _spriteRenderer2;


    public bool CanMoveToNode(Node node)
    {
        // Validate this prophet can move the distance to the given node
        return currentNode.IsNeighbor(node);
    }

    public bool MoveProphet(Node newNode)
    {

        // Move the prophet to the new node location
        if (CanMoveToNode(newNode))
        {
            StartCoroutine(DoMoveProphet(newNode.transform.position));
            CurrentNode = newNode;
            return true;
        }
        return false;
    }

    private IEnumerator DoMoveProphet(Vector3 newLocation)
    {
        float speed = 2f;
        while (transform.position != newLocation)
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, newLocation, step);
            yield return null;
        }
    }

    public override void Evangelize()
    {
        neighbors = currentNode.neighbors;
        base.Evangelize();
    }

    public override void UpdateHealth()
    {
        base.UpdateHealth();
        if (Leader != null)
        {
            if (CurrentHealth < OwnershipThreshold)
            {
                Leader.Prophets.Remove(this);
            } else
            {
                if (!Leader.Prophets.Contains(this))
                {
                    Leader.Prophets.Add(this);
                }
            }
        }
	}

	protected override bool IsFloor { get { return false; } }
}
