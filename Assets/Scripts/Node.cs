using MakingWaves.Model;
using System.Collections.Generic;
using UnityEngine;

public class Node : Mind
{
    // TODO:
    //  - move hex logic into board, or even special manager
    //  - encapsulation of various fields/properties
    //  - ideally, some separation of game logic and presentation/engine interaction

    #region Vectors to neighbors
    public static readonly float ROOT_3 = Mathf.Sqrt(3);
    public static readonly Vector2 TO_12_OCLOCK = new Vector2(0, ROOT_3 / 2);
    public static readonly Vector2 TO_2_OCLOCK = new Vector2(0.75f, ROOT_3 / 4);
    public static readonly Vector2 TO_4_OCLOCK = Vector2.Scale(TO_2_OCLOCK, new Vector2(1, -1));
    public static readonly Vector2 TO_6_OCLOCK = TO_12_OCLOCK;
    public static readonly Vector2 TO_8_OCLOCK = Vector2.Scale(TO_2_OCLOCK, new Vector2(-1, -1));
    public static readonly Vector2 TO_10_OCLOCK = Vector2.Scale(TO_2_OCLOCK, new Vector2(-1, 1));

    public static readonly IDictionary<int, Vector2> DIRECTIONS = new Dictionary<int, Vector2>()
    {
        { 0, TO_12_OCLOCK },
        { 2, TO_2_OCLOCK },
        { 4, TO_4_OCLOCK },
        { 6, TO_6_OCLOCK },
        { 8, TO_8_OCLOCK },
        { 10, TO_10_OCLOCK }
    };
    #endregion

    #region UI
    private SpriteRenderer spriteRenderer;
    // TODO: work on "status"
    private SpriteRenderer neutralSpriteRenderer;
    private TextMesh debugText;
    public virtual string DebugText { get { return debugText.text; } set { debugText.text = value; } }
    #endregion

    #region Game Loop/Events
    protected virtual void Awake()
    {
        Prophets = new List<Prophet>();
        debugText = GetComponentInChildren<TextMesh>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        //neutralSpriteRenderer = transform.Find("NeutralTile").GetComponent<SpriteRenderer>();
    }
    #endregion

    #region Spatial
    public IList<Node> neighbors = new List<Node>();

    public IList<Prophet> Prophets { get; set; }

    public void ComputeNeighbors()
    {
        var collider = GetComponent<PolygonCollider2D>();
        foreach (var clockDirection in DIRECTIONS)
        {
            // find adjacent nodes, but only going left to right
            if (clockDirection.Key >= 6) continue;
            collider.enabled = false;
            var hit = Physics2D.Raycast(transform.position, clockDirection.Value, 1f);
            collider.enabled = true;
            if (hit.transform == null) continue;

            // create an edge from this to that and connect each object together
            var otherNode = hit.transform.gameObject.GetComponent<Node>();
            var edge = new Edge(this, otherNode, clockDirection.Value);
            edges[clockDirection.Value] = edge;
            var oppositeDirection = DIRECTIONS[(clockDirection.Key + 6) % 12];
            otherNode.edges[oppositeDirection] = edge;

            neighbors.Add(otherNode);
            otherNode.neighbors.Add(this);
        }
    }

    public bool IsNeighbor(Node node)
    {
        return neighbors.Contains(node);
    }

    public Vector2 GridPosition { get; set; }

    private IDictionary<Vector2, Edge> edges = new Dictionary<Vector2, Edge>();

    public override bool Equals(object obj)
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

    public class Edge
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
            if (direction == LeftToRight)
            {
                return Right;
            }
            else
            {
                return Left;
            }
        }
    }
    #endregion

    #region Influence Mechanics
    public IList<Mind> GetInfluencers()
    {
        // TODO: influencers include current node + neighbors, and prophets in each of those.
        IList<Mind> influencers = new List<Mind>();
        foreach (var prophet in Prophets)
        {
            influencers.Add(prophet);
        }
        foreach (var neighbor in neighbors)
        {
            influencers.Add(neighbor);
            foreach (var prophet in neighbor.Prophets)
            {
                influencers.Add(prophet);
            }
        }
        return influencers;
    }

    public override void ApplyInfluence()
    {
        base.ApplyInfluence();
        UpdateOwner();
    }

    public override void SetOwner(Player player)
    {
        base.SetOwner(player);
        UpdateOwner();
    }

    private void UpdateOwner()
    {
        if (IsOwned)
        {
            var partOwned = Mathf.Min(currentHealth / ownershipThreshold, 1);
            spriteRenderer.sprite = Owner.Idea.ownedNodeSprite;
            //neutralSpriteRenderer.color = neutralSpriteRenderer.color.WithAlpha(1 - partOwned);
            //MySpriteRenderer.color = MySpriteRenderer.color.WithAlpha(partOwned);

            DebugText = currentHealth > 0 ? string.Format("{0:g2}", currentHealth) : "";
            debugText.color = Owner == null ? Color.black : Owner.Idea.color;
        }
    }
    #endregion

    public void TestNode(bool color)
    {
        var nodeSpriteRenderer = GetComponent<SpriteRenderer>();
        nodeSpriteRenderer.color = color ? Color.blue : Color.white;
        //		foreach (var connection in edges) {
        //			var edge = connection.Value;
        //			var otherSpriteRenderer = edge.Follow(connection.Key).gameObject.GetComponent<SpriteRenderer> ();
        //			otherSpriteRenderer.color = color ? Color.red : Color.white;
        //		}

        foreach (var n in neighbors)
        {
            var otherSpriteRenderer = n.gameObject.GetComponent<SpriteRenderer>();
            otherSpriteRenderer.color = color ? Color.red : Color.white;
        }
    }
}
