using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	/// <summary>
	/// The nodes owned by this player
	/// </summary>
	private Dictionary<Vector3, Node> nodes = new Dictionary<Vector3, Node>();

	private List<Prophet> prophets = new List<Prophet> ();
	public List<Prophet> Prophets {
		get {
			return prophets;
		}
		set {
			prophets = value;
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
