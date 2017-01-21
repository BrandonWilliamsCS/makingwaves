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
	/// <summary>
	/// Gets the health threshold.
	/// This is how many influence points it takes before we can emit influence
	/// </summary>
	/// <value>The health threshold.</value>
	public int HealthThreshold {
		get {
			return healthThreshold;
		}
	}

	[SerializeField]
	private int currentHealth = 0;
	/// <summary>
	/// Gets the current health.
	/// Current health 0 denotes a neutral node
	/// </summary>
	/// <value>The current health.</value>
	public int CurrentHealth {
		get {
			return currentHealth;
		}
	}

	[SerializeField]
	private int influenceWeight = 1;
	public int InfluenceWeight {
		get {
			return influenceWeight;
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void EmitInfluence() {
		// TODO fire particle effect
	}
}
