using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour {

	[SerializeField]
	private Player Owner;

	[SerializeField]
	private int _topHealth = 10;
	public int TopHealth {
		get {
			return _topHealth;
		}
	}

	[SerializeField]
	private int _healthThreshold = 7;
	/// <summary>
	/// Gets the health threshold.
	/// This is how many influence points it takes before we can emit influence
	/// </summary>
	/// <value>The health threshold.</value>
	public int HealthThreshold {
		get {
			return _healthThreshold;
		}
	}

	[SerializeField]
	private int _currentHealth = 0;
	/// <summary>
	/// Gets the current health.
	/// Current health 0 denotes a neutral node
	/// </summary>
	/// <value>The current health.</value>
	public int CurrentHealth {
		get {
			return _currentHealth;
		}
	}

	[SerializeField]
	private int _influenceWeight = 1;
	public int InfluenceWeight {
		get {
			return _influenceWeight;
		}
	}

	private Vector2 _gridPosition;
	public Vector2 GridPosition {
		get {
			return _gridPosition;
		}
		set {
			_gridPosition = value;
		}
	}

	private ParticleSystem _influenceEmitter;
	private ParticleSystem InfluenceEmitter {
		get {
			if (_influenceEmitter == null) {
				_influenceEmitter = GetComponentInChildren<ParticleSystem> ();
			}
			return _influenceEmitter;
		}
	}

	private SpriteRenderer _spriteRenderer;
	private SpriteRenderer MySpriteRenderer {
		get {
			if (_spriteRenderer == null) {
				_spriteRenderer = this.GetComponent<SpriteRenderer> ();
			}
			return _spriteRenderer;
		}
	}

	// Use this for initialization
	void Start () {
		EmitInfluence ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void EmitInfluence() {
		if (InfluenceEmitter != null) {
			if (MySpriteRenderer != null) {
				var main = InfluenceEmitter.main;
				main.startColor = MySpriteRenderer.color;
			}
			InfluenceEmitter.Play();
		}
	}
}
