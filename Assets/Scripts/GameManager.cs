using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

	[SerializeField]
	private GameObject nodePrefab;

	/// <summary>
	/// All the players in the game
	/// </summary>
	List<Player> players = new List<Player>();

	// Use this for initialization
	void Start () {
		// Call the NodeGenerator to generate all the node game objects into the scene

		Player player1 = new GameObject ("Player").AddComponent<Player> ();
		players.Add (player1);

		Prophet prophet = FindObjectOfType<Prophet> ();
		player1.Prophets.Add (prophet);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonUp (0)) {
			RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
			Node node = hit.transform.GetComponent<Node> ();
			if (node != null) {
				players [0].Prophets [0].MoveProphet (node);
			}
		}
	}

	void OnDestroy() {
		instance = null;
	}

	public void NodeChangingInfluence(Node node) {
		// Check the Nodes current health - if 0 set it into the neutralList
		// If current health > 0, move from the neutralList into the Node.Player.Nodes list.
	}
}
