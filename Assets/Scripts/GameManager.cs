using MakingWaves.Model;
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
    [SerializeField]
    private GameObject boardManagerPrefab;
    public Idea[] ideas;
    #endregion

    #region UI
    [SerializeField]
    private GameObject victoryPanel;
    [SerializeField]
    private GameObject readyOverlay;
    private Image readyOverlayImage; // TODO: this would go well in a specialized "UI" class, along with the victory panel and scores.
    #endregion

    public float victoryScore = 10;
    private bool gameOver;
    private BoardManager board;
    private int currentPlayer;
    private bool playerMoved; // TODO: player turn state to be encapsulated

    /// <summary>
    /// All the players in the game
    /// </summary>
    private List<Player> players = new List<Player>();

    #region Game Loop/Events
    void Awake()
    {
        board = Instantiate(boardManagerPrefab).GetComponent<BoardManager>();
        readyOverlayImage = readyOverlay.GetComponent<Image>();
    }

    void Start()
    {
        // TODO: consider improving neutral logic here.
        MakePlayer(Vector2.left);
        foreach (var start in board.PlayerStarts)
        {
            MakePlayer(start);
        }
        SetButtonColor(ideas[currentPlayer + 1].color); // TODO: this will naturally be a communication between turn state and UI.
    }

    void Update()
    {
        if (!gameOver && Input.GetMouseButtonUp(0))
        {
            MoveProphetOnTurn();
        }
    }

    void OnDestroy()
    {
        _instance = null;
    }
    #endregion

    #region Initial player creation
    private Player MakePlayer(Vector2 start)
    {
        // TODO: allow multiple starting prophets, "owned" locations? Differentiate player data and engine behavior
        var playerObject = Instantiate(playerPrefab);
        var player = playerObject.GetComponent<Player>();
        player.Idea = ideas[players.Count];
        if (start.x >= 0)
        {
            var node = board.GetNodeAt(start);
            player.InitializeProphets(node);
            // TODO: UI
            player.ScoreDisplay = GameObject.Find(player.Name + "Score").GetComponent<Text>();
        }
        else
        {
            player.InitializeProphets(null);
        }

        players.Add(player);
        return player;
    }
    #endregion

    private void MoveProphetOnTurn()
    {
        // TODO: move to separate function(s), coordinate with turn state
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

    public void OnReadyPressed()
    {
        // TODO: integrate with turn state, UI
        currentPlayer = (currentPlayer + 1) % (players.Count - 1);
        playerMoved = false;
        SetButtonColor(ideas[currentPlayer + 1].color);
        // if we haven't cylced back to red's turn, don't compute
        // TODO: Instead of waiting for a cycle, wait until each player is ready.
        if (currentPlayer != 0) return;

        ProcessInfluence();
    }

    private void ProcessInfluence()
    {
        foreach (var node in board.Nodes.Values)
        {
            node.AcceptInfluence();
        }
        foreach (var player in players)
        {
            foreach (var prophet in player.Prophets)
            {
                prophet.AcceptInfluence();
            }
        }
        foreach (var node in board.Nodes.Values)
        {
            node.ApplyInfluence();
        }
        foreach (var player in players)
        {
            foreach (var prophet in player.Prophets)
            {
                prophet.ApplyInfluence();
            }
        }

        ProcessScores();
    }

    // TODO: UI manager
    private void SetButtonColor(Color color)
    {
        readyOverlayImage.color = color.WithAlpha(0.5f);
    }

    // TODO: use UI manager
    public void ProcessScores()
    {
        var maxScore = 0f;
        Player leadingPlayer = null;
        foreach (var player in players)
        {
            if (player.ScoreDisplay != null)
                player.ScoreDisplay.text = "Score: " + player.Score;
            if (player.Score > maxScore)
            {
                maxScore = player.Score;
                leadingPlayer = player;
            }
        }
        if (maxScore > victoryScore)
        {
            victoryPanel.SetActive(true);
            GameObject.Find("WinText").GetComponent<Text>().text = leadingPlayer.Name + " Wins!";
            gameOver = true;
        }
    }
}

static class Extensions
{
    public static Color WithAlpha(this Color color, float alpha)
    {
        return new Color(color.r, color.g, color.b, alpha);
    }
}