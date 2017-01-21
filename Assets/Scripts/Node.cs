using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour {

//	[SerializeField]
//	private Player owner;

	[SerializeField]
	private int topHealth = 10;
	public int TopHealth {
		get {
			return topHealth;
		}
	}

	[SerializeField]
	private int healthThreshold = 7;
	public int HealthThreshold {
		get {
			return healthThreshold;
		}
	}

	[SerializeField]
	private int currentHealth = 0; // Current health 0 denotes a neutral node
	public int CurrentHealth {
		get {
			return currentHealth;
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
