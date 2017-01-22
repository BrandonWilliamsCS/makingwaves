using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    [SerializeField]
    private float _conversionRateMultiplier = 1.0f;
    /// <summary>
    /// Gets the player multiplier for evangalizing.
    /// </summary>
    /// <value>The current health.</value>
    public float ConversionRateMultiplier
    {
        get
        {
            return _conversionRateMultiplier;
        }
    }

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

	private Color color;
	public Color Color { 
		get { return color; }
		set {
			color = value;
		}
	}

	public Sprite TileSprite { get; set; }

	public string Name {get;set;}
	public Text ScoreDisplay {get;set;}

    private float score;
    public float Score {
		get { return score; }
		set {
			score = value;
			if (ScoreDisplay != null)
				ScoreDisplay.text = "Score: " + value;
			if (score > 1000) {
				GameManager.Instance.Winner (this);
			}
		}
	}
}
