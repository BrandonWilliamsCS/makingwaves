using System.Collections;
using UnityEngine;

public class Prophet : Node //!! TODO: Probably switch to a common (abstract?) base class here. Other cleanup mostly shakes out from there.
{
    // TODO: various presentation ideas for movement status

    private Node currentNode;
    public Node CurrentNode
    {
        get { return currentNode; }
        set
        {
            if (currentNode != null)
            {
                currentNode.Prophets.Remove(this);
            }
            currentNode = value;
            currentNode.Prophets.Add(this);
        }
    }

    protected override void Awake()
    {
        base.Awake();
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

    public override void AcceptInfluence()
    {
        //!! TODO: better way to disable
        if (!gameObject.activeInHierarchy) return;
        neighbors = currentNode.neighbors;
        base.AcceptInfluence();
    }

    public override void ApplyInfluence()
    {
        base.ApplyInfluence();
        if (Leader != null)
        {
            Leader.UpdateProphetOwnership(this, IsOwned);
        }
    }

    protected override bool IsFloor { get { return false; } }
}
