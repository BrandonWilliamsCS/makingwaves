using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
			foreach (var prophet in prophets)
				prophet.Color = color;
		}
	}

	private Color color;
	public Color Color { 
		get { return color; }
		set {
			color = value;
			foreach (var prophet in prophets)
				prophet.Color = value;
		}
	}

	public Sprite TileSprite { get; set; }

    private float score;
    public float Score { get { return score; } set { score = value; } }
}
