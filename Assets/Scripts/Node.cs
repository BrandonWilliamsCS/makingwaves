using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{

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
    private Player Leader;

    [SerializeField]
    private float _topHealth = 10f;
    public float TopHealth
    {
        get
        {
            return _topHealth;
        }
    }

    [SerializeField]
    private float _evangelismThreshold = 7f;
    /// <summary>
    /// Gets the health threshold.
    /// This is how many influence points it takes before we can emit influence
    /// </summary>
    /// <value>The health threshold.</value>
    public float EvangelismThreshold
    {
        get
        {
            return _evangelismThreshold;
        }
    }

    [SerializeField]
    private float _ownershipThreshold = 3f;
    /// <summary>
    /// Gets the health threshold.
    /// This is how many influence points it takes before we can emit influence
    /// </summary>
    /// <value>The health threshold.</value>
    public float OwnershipThreshold
    {
        get
        {
            return _ownershipThreshold;
        }
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
    }
    private float _calculatedHealth = 0f;

    [SerializeField]
    private float _conversionStrength = 1f;
    public float ConversionStrength
    {
        get
        {
            return _conversionStrength;
        }
    }

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
    private SpriteRenderer MySpriteRenderer
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

    // Use this for initialization
    void Start()
    {
        //Evangelize ();
    }

    // Update is called once per frame
    void Update()
    {

    }

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
        }
    }

    public void Evangelize()
    {
        if (InfluenceEmitter != null)
        {
            if (MySpriteRenderer != null)
            {
                var main = InfluenceEmitter.main;
                main.startColor = MySpriteRenderer.color;
            }
            InfluenceEmitter.Play();
        }

        //Calculate neighbor evangelism and control
        _calculatedHealth = CurrentHealth;
        float neutralConversionRateMultiplier = .1f;
        if (_calculatedHealth > 0f)
        {
            foreach (var edgePair in edges)
            {
                Node neighbor = edgePair.Value.Follow(edgePair.Key);
                if (neighbor.CanEvangelize)
                {
                    if (Leader == neighbor.Leader)
                    {
                        _calculatedHealth += neighbor.Leader.ConversionRateMultiplier * neighbor.ConversionStrength;
                    }
                    else
                    {
                        _calculatedHealth -= neighbor.Leader.ConversionRateMultiplier * neighbor.ConversionStrength;
                    }
                }
                else
                {
                    if (!neighbor.IsOwned)
                    {
                        _calculatedHealth -= neutralConversionRateMultiplier * neighbor.ConversionStrength;
                    }
                }
            }
        }
        else
        {
            IDictionary<Player, float> influenceScore = new Dictionary<Player, float>();
            float neutralInfluence = 0;
            foreach (var edgePair in edges)
            {
                Node neighbor = edgePair.Value.Follow(edgePair.Key);
                if (neighbor.CanEvangelize)
                {
                    if (influenceScore.ContainsKey(neighbor.Leader))
                    {
                        influenceScore[neighbor.Leader] += neighbor.Leader.ConversionRateMultiplier * neighbor.ConversionStrength;
                    }
                    else
                    {
                        influenceScore[neighbor.Leader] = neighbor.Leader.ConversionRateMultiplier * neighbor.ConversionStrength;
                    }
                }
                else
                {
                    if (!neighbor.IsOwned)
                    {
                        neutralInfluence += neutralConversionRateMultiplier * neighbor.ConversionStrength;
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
                Leader = effectiveLeader;
                //Leader.nodes.add(this);
            }
        }
    }

    public bool IsOwned { get { return CurrentHealth >= OwnershipThreshold; } }

    public bool CanEvangelize { get { return CurrentHealth >= EvangelismThreshold; } }

    public void updateHealth()
    {
        if (_calculatedHealth > TopHealth)
        {
            _calculatedHealth = TopHealth;
        }
        else if (_calculatedHealth < 0)
        {
            _calculatedHealth = 0;
        }
        _currentHealth = _calculatedHealth;
    }

    #region Model
    public Vector2 GridPosition { get; set; }

    //!! TODO make private
    public IDictionary<Vector2, Edge> edges = new Dictionary<Vector2, Edge>();

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
}
