using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region Singleton
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<GameManager>();
            }
            if (_instance == null)
            {
                _instance = new GameObject().AddComponent<GameManager>();
            }
            return _instance;
        }
    }
    #endregion

    #region Unity Injected
    // set from editor
    [SerializeField]
    private GameObject playerPrefab;
    //!! TODO: create class for these UI injections
    public Color[] playerColors;
    public Sprite[] prophetSprites;
    public Sprite[] ownerTiles;
    public string[] playerNames;
    #endregion

    #region UI
    [SerializeField]
    private GameObject victoryPanel;
    private Image buttonOverlay; //!! TODO: this would go well in a specialized "UI" class, along with the victory panel and scores.
    #endregion

    private bool gameOver;
    private BoardManager board;
    public IList<Vector2> PlayerStarts { get; set; } //!! TODO: move to board manager, and let "current" player refs stay
    private int currentPlayer;
    private bool playerMoved; // TODO: player turn state to be encapsulated

    /// <summary>
    /// All the players in the game
    /// </summary>
    List<Player> players = new List<Player>();

    #region Game Loop/Events
    void Awake()
    {
        //!! TODO: BM should probably be a prefab that gets instantiated. For now, maybe just inject this too.
        board = FindObjectOfType<BoardManager>();
        //!! TODO: inject ReadyTint, only get image component here?
        buttonOverlay = GameObject.Find("ReadyTint").GetComponent<Image>();
    }

    void Start()
    {
        // TODO: consider improving neutral logic here.
        MakePlayer(Vector2.left);
        foreach (var start in PlayerStarts)
        {
            MakePlayer(start);
        }
        SetButtonColor(playerColors[currentPlayer + 1]); // TODO: this will naturally be a communication between turn state and UI.
        GivePlayersHomes(); //!! incorporate into MakePlayer based on starting location
    }

    void Update()
    {
        //!! TODO: move to separate function(s), coordinate with turn state 
        if (!gameOver && Input.GetMouseButtonUp(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.transform)
            {
                Node node = hit.transform.GetComponent<Node>();
                var canMove = !playerMoved && players[currentPlayer + 1].Prophets[0].MoveProphet(node);
                if (canMove)
                {
                    playerMoved = true;
                }
            }
        }
    }

    void OnDestroy()
    {
        _instance = null;
    }
    #endregion

    private Player MakePlayer(Vector2 start)
    {
        //!! TODO: much of this probably belongs in player.
        // TODO: allow multiple starting prophets, "owned" locations?
        var playerObject = Instantiate(playerPrefab);
        var player = playerObject.GetComponent<Player>();
        player.Color = playerColors[players.Count];
        player.TileSprite = ownerTiles[players.Count];
        player.Name = playerNames[players.Count];

        // deal with prophets
        Prophet prophet = player.GetComponentInChildren<Prophet>();
        prophet.GetComponentInChildren<SpriteRenderer>().sprite = prophetSprites[players.Count];
        prophet.Leader = player;
        prophet.CurrentHealth = prophet.TopHealth;
        if (start.x >= 0)
        {
            player.ScoreDisplay = GameObject.Find(player.Name + "Score").GetComponent<Text>();
            var node = board.GetNodeAt(start);
            prophet.CurrentNode = node;
            prophet.transform.position = node.transform.position;
            player.Prophets.Add(prophet);
        }
        else
        {
            prophet.gameObject.SetActive(false);
        }

        players.Add(player);
        return player;
    }

    public void OnReadyPressed()
    {
        // TODO: integrat with turn state
        currentPlayer = (currentPlayer + 1) % (players.Count - 1);
        playerMoved = false;
        SetButtonColor(playerColors[currentPlayer + 1]);
        // if we haven't cylced back to red's turn, don't compute
        if (currentPlayer != 0)
            return;

        //!! TODO: move into separate functions, possibly in other classes
        foreach (var nodePair in board.Nodes)
        {
            nodePair.Value.Evangelize();
        }
        foreach (var player in players)
        {
            foreach (var prophet in player.Prophets)
            {
                prophet.Evangelize();
            }
        }
        foreach (var nodePair in board.Nodes)
        {
            nodePair.Value.UpdateHealth();
        }
        foreach (var player in players)
        {
            foreach (var prophet in player.Prophets)
            {
                prophet.UpdateHealth();
            }
        }
    }

    //!! TODO: UI manager
    private void SetButtonColor(Color color)
    {
        buttonOverlay.color = color.WithAlpha(0.5f);
    }

    //!! TODO: use or delete this!
    public void NodeChangingInfluence(Node node)
    {
        // Check the Nodes current health - if 0 set it into the neutralList
        // If current health > 0, move from the neutralList into the Node.Player.Nodes list.
    }

    //!! TODO: better compute winner logic, use UI manager, 
    public void Winner(Player player)
    {
        victoryPanel.SetActive(true);
        GameObject.Find("WinText").GetComponent<Text>().text = player.Name + " Wins!";
        gameOver = true;
    }

    //!! TODO: this is very hacky atm.
    public void GivePlayersHomes()
    {
        var node = board.GetNodeAt(new Vector2(0, 0));
        node.TestInteraction(players[1], 10);
        node.CurrentHealth = node.CurrentHealth;

        board.GetNodeAt(new Vector2(9, 9)).TestInteraction(players[2], 10);
        node.TestInteraction(players[1], 10);
        node.CurrentHealth = node.CurrentHealth;

        board.GetNodeAt(new Vector2(0, 9)).TestInteraction(players[3], 10);
        node.TestInteraction(players[1], 10);
        node.CurrentHealth = node.CurrentHealth;

        board.GetNodeAt(new Vector2(9, 0)).TestInteraction(players[4], 10);
        node.TestInteraction(players[1], 10);
        node.CurrentHealth = node.CurrentHealth;
    }
}

static class Extensions
{
    public static Color WithAlpha(this Color color, float alpha)
    {
        return new Color(color.r, color.g, color.b, alpha);
    }
}