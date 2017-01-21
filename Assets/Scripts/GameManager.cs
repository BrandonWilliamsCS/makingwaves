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
	[SerializeField]
	List<Player> players = new List<Player>();

	// Use this for initialization
	void Start () {
		// Call the NodeGenerator to generate all the node game objects into the scene
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnDestroy() {
		instance = null;
	}

	public void NodeChangingInfluence(Node node) {
		// Check the Nodes current health - if 0 set it into the neutralList
		// If current health > 0, move from the neutralList into the Node.Player.Nodes list.
	}
}
