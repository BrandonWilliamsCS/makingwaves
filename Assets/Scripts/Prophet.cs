using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prophet : Node
{
    public override string DebugText
    {
        get
        {
            return "";
        }

        set
        {
            ;
        }
    }

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

	private SpriteRenderer _spriteRenderer;
	protected override SpriteRenderer MySpriteRenderer
	{
		get
		{
			if (_spriteRenderer == null)
			{
				_spriteRenderer = this.GetComponentInChildren<SpriteRenderer>();
			}
			return _spriteRenderer;
		}
	}

	public Color Color { get { return MySpriteRenderer.color; } set { MySpriteRenderer.color = value; } }

    // Update is called once per frame
    void Update()
    {

    }

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
}
