using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	private static GameManager instance;
	public static GameManager Instance {
		get {
			if(instance == null) {
				instance = GameObject.FindObjectOfType<GameManager> ();
			}
			if (instance == null) {
				instance = new GameObject ().AddComponent<GameManager> ();
			}
			return instance;
		}
	}

	public static readonly Color[] COLORS_DEP = new Color[] {
		Color.gray,
		Color.red,
		Color.green,
		Color.blue,
		Color.magenta,
	};

	// set from editor
	[SerializeField]
	private GameObject playerPrefab;
	public Color[] playerColors;
	public Sprite[] prophetSprites;
	public Sprite[] ownerTiles;
	public string[] playerNames;

	private BoardManager board;
	private Image buttonOverlay;
	public IList<Vector2> PlayerStarts { get; set; }
	private int currentPlayer;
	private bool playerMoved;

	/// <summary>
	/// All the players in the game
	/// </summary>
	List<Player> players = new List<Player>();

	private GameObject vp;
	void Awake () {
		board = GameObject.FindObjectOfType<BoardManager> ();
		buttonOverlay = GameObject.Find ("ReadyTint").GetComponent<Image>();
		vp = GameObject.Find ("VictoryPanel");
			vp.SetActive(false);
	}

	void Start () {
		MakePlayer (Vector2.left);
		foreach (var start in PlayerStarts)
		{
			MakePlayer (start);
		}
		SetButtonColor (playerColors[currentPlayer + 1]);
		GivePlayersHomes (); //!!
	}

	private Player MakePlayer(Vector2 start)
	{
		var playerObject = Instantiate(playerPrefab);
		var player = playerObject.GetComponent<Player>();
		player.Color = playerColors[players.Count];
		player.TileSprite = ownerTiles[players.Count];
		player.Name = playerNames[players.Count];

        // deal with prophets
        Prophet prophet = player.GetComponentInChildren<Prophet>();
		prophet.GetComponentInChildren<SpriteRenderer> ().sprite = prophetSprites [players.Count];
        prophet.Leader = player;
        prophet.CurrentHealth = prophet.TopHealth;
        if (start.x >= 0)
		{
			player.ScoreDisplay = GameObject.Find (player.Name + "Score").GetComponent<Text>();
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

	void Update () {
		if (!end && Input.GetMouseButtonUp (0)) {
			RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
			if (hit.transform) {
				Node node = hit.transform.GetComponent<Node> ();
				var canMove = !playerMoved && players [currentPlayer + 1].Prophets [0].MoveProphet (node);
				if (canMove) {
					playerMoved = true;
				}
			}
		}
	}

	public void OnReadyPressed()
	{
		currentPlayer = (currentPlayer + 1) % (players.Count - 1);
		playerMoved = false;
		SetButtonColor (playerColors[currentPlayer + 1]);
		// if we haven't cylced back to red's turn, don't compute
		if (currentPlayer != 0)
			return;
        foreach (var nodePair in board.nodes)
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
        foreach (var nodePair in board.nodes)
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

	private void SetButtonColor(Color color)
	{
		buttonOverlay.color = color.WithAlpha(0.5f);
	}

	void OnDestroy() {
		instance = null;
	}

	public void NodeChangingInfluence(Node node) {
		// Check the Nodes current health - if 0 set it into the neutralList
		// If current health > 0, move from the neutralList into the Node.Player.Nodes list.
	}

	private bool end;
	public void Winner(Player player)
	{
		vp.SetActive(true);
		GameObject.Find ("WinText").GetComponent<Text>().text = player.Name + " Wins!";
		end = true;
	}

	public void GivePlayersHomes()
	{
		var node = board.GetNodeAt (new Vector2 (0, 0));
		node.TestInteraction(players[1], 10);
		node.CurrentHealth = node.CurrentHealth;

		board.GetNodeAt (new Vector2(9, 9)).TestInteraction(players[2], 10);
		node.TestInteraction(players[1], 10);
		node.CurrentHealth = node.CurrentHealth;

		board.GetNodeAt (new Vector2(0, 9)).TestInteraction(players[3], 10);
		node.TestInteraction(players[1], 10);
		node.CurrentHealth = node.CurrentHealth;

		board.GetNodeAt (new Vector2(9, 0)).TestInteraction(players[4], 10);
		node.TestInteraction(players[1], 10);
		node.CurrentHealth = node.CurrentHealth;
	}
}

static class Extensions
{
	public static Color WithAlpha(this Color color, float alpha) {
					return new Color (color.r, color.g, color.b, alpha);
	}
}