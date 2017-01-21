using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Influencer : MonoBehaviour {

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
