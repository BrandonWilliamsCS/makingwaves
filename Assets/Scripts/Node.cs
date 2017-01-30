using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    //!! TODO: this whole class can be cleaned up.
    //  - no per-node serializaton needed, since this won't be directly accessible to editor anyway
    //  - many fields/props can be one or the other
    //  - establish _ meaining
    //  - get components in Awake for consistency, namespace cleaning
    //  - hard-coded values should be gotten elsewhere (unity and/or in-game editable, idea-based)
    //  - move hex logic into board, or even special manager
    //  - code regions
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

    #region Configurable
    // TODO: better option than storing defaults here?
    [SerializeField]
    private float topHealth = 10f;

    /// <summary>
    /// Gets the health threshold.
    /// This is how many influence points it takes before we can emit influence
    /// </summary>
    /// <value>The health threshold.</value>
    [SerializeField]
    private float evangelismThreshold = 7f;

    /// <summary>
    /// Gets the health threshold.
    /// This is how many influence points it takes before we can emit influence
    /// </summary>
    /// <value>The health threshold.</value>
    [SerializeField]
    private float ownershipThreshold = 3f;

    [SerializeField]
    private float conversionStrength = 1f;
    #endregion

    #region UI
    private TextMesh debugText;
    public virtual string DebugText { get { return debugText.text; } set { debugText.text = value; } }

    // TODO: these need work.
    private ParticleSystem _influenceEmitter;
    private ParticleSystem InfluenceEmitter
    {
        get
        {
            if (_influenceEmitter == null)
            {
                _influenceEmitter = GetComponentInChildren<ParticleSystem>();
            }
            return _influenceEmitter;
        }
    }
    private SpriteRenderer _spriteRenderer;
    protected virtual SpriteRenderer MySpriteRenderer
    {
        get
        {
            if (_spriteRenderer == null)
            {
                _spriteRenderer = this.GetComponent<SpriteRenderer>();
            }
            return _spriteRenderer;
        }
    }
    // TODO: work on "status"
    private SpriteRenderer neutralSpriteRenderer;

    private void Emit()
    {
        if (InfluenceEmitter != null)
        {
            if (MySpriteRenderer != null)
            {
                var main = InfluenceEmitter.main;
                main.startColor = Leader.Idea.color;
            }
            InfluenceEmitter.Play();
        }
    }
    #endregion

    #region Game Loop/Events
    protected virtual void Awake()
    {
        Prophets = new List<Prophet>();
        debugText = GetComponentInChildren<TextMesh>();
        //neutralSpriteRenderer = transform.Find("NeutralTile").GetComponent<SpriteRenderer>();
    }
    #endregion

    #region Game Mechanics
    private Player _leader;
    public Player Leader
    {
        get { return _currentHealth > 0 ? _leader : null; }
        set { _leader = value; }
    }

    [SerializeField]
    private float _currentHealth = 0f;
    /// <summary>
    /// Gets the current health.
    /// </summary>
    /// <value>The current health.</value>
    public float CurrentHealth
    {
        get
        {
            return _currentHealth;
        }
        set
        {
            _currentHealth = value;
            if (IsFloor && IsOwned)
            {
                var partOwned = Mathf.Min(_currentHealth / ownershipThreshold, 1);
                MySpriteRenderer.sprite = Leader.Idea.ownedNodeSprite;
                //neutralSpriteRenderer.color = neutralSpriteRenderer.color.WithAlpha(1 - partOwned);
                //MySpriteRenderer.color = MySpriteRenderer.color.WithAlpha(partOwned);
            }
        }
    }
    private float _calculatedHealth = 0f;

    //!! TODO: looooong method. break into sub-functions, and ideally prep for configuration or mechanic change.
    public virtual void AcceptInfluence()
    {
        if (_currentHealth > evangelismThreshold)
        {
            Emit();
            Leader.Score += Leader.ConversionRateMultiplier * conversionStrength;
        }

        //Calculate neighbor evangelism and control
        IList<Node> influencers = new List<Node>();
        foreach (var prophet in this.Prophets)
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
        _calculatedHealth = CurrentHealth;
        float neutralConversionRateMultiplier = .2f;
        if (_calculatedHealth > 0f)
        {
            foreach (var influencer in influencers)
            {
                if (influencer.CanEvangelize)
                {
                    if (_leader == influencer._leader)
                    {
                        _calculatedHealth += influencer._leader.ConversionRateMultiplier * influencer.conversionStrength;
                    }
                    else
                    {
                        _calculatedHealth -= influencer._leader.ConversionRateMultiplier * influencer.conversionStrength;
                    }
                }
                else
                {
                    if (!influencer.IsOwned)
                    {
                        _calculatedHealth -= neutralConversionRateMultiplier * influencer.conversionStrength;
                    }
                }
            }
        }
        else
        {
            IDictionary<Player, float> influenceScore = new Dictionary<Player, float>();
            float neutralInfluence = 0;
            foreach (var influencer in influencers)
            {
                if (influencer.CanEvangelize)
                {
                    if (influenceScore.ContainsKey(influencer._leader))
                    {
                        influenceScore[influencer._leader] += influencer._leader.ConversionRateMultiplier * influencer.conversionStrength;
                    }
                    else
                    {
                        influenceScore[influencer._leader] = influencer._leader.ConversionRateMultiplier * influencer.conversionStrength;
                    }
                }
                else
                {
                    if (!influencer.IsOwned)
                    {
                        neutralInfluence += neutralConversionRateMultiplier * influencer.conversionStrength;
                    }
                }
            }
            Player effectiveLeader = null;
            foreach (var scorePair in influenceScore)
            {
                if (scorePair.Value > _calculatedHealth)
                {
                    effectiveLeader = scorePair.Key;
                    _calculatedHealth = scorePair.Value;
                }
            }
            foreach (var scorePair in influenceScore)
            {
                if (scorePair.Key != effectiveLeader)
                {
                    _calculatedHealth -= scorePair.Value;
                }
            }
            _calculatedHealth -= neutralInfluence;
            if (_calculatedHealth > 0)
            {
                //Leader.nodes.remove(this);
                _leader = effectiveLeader;
                //Leader.nodes.add(this);
            }
        }
    }

    public bool IsOwned { get { return CurrentHealth >= ownershipThreshold; } }

    public bool CanEvangelize { get { return CurrentHealth >= evangelismThreshold; } }

    public virtual void ApplyInfluence()
    {
        if (_calculatedHealth > topHealth)
        {
            _calculatedHealth = topHealth;
        }
        else if (_calculatedHealth < 0)
        {
            _calculatedHealth = 0;
        }
        CurrentHealth = _calculatedHealth;

        DebugText = _currentHealth > 0 ? string.Format("{0:g2}", _currentHealth) : "";
        debugText.color = _leader == null ? Color.black : _leader.Idea.color;
    }
    #endregion

    public IList<Prophet> Prophets { get; set; }

    #region Spatial
    public IList<Node> neighbors = new List<Node>();

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

    //!! TODO: nuff said.
    protected virtual bool IsFloor { get { return true; } }

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

    public void SetOwner(Player player)
    {
        Leader = player;
        CurrentHealth = topHealth;
        DebugText = _currentHealth > 0 ? string.Format("{0:g2}", _currentHealth) : "";
        debugText.color = _leader == null ? Color.black : _leader.Idea.color;
    }
}
