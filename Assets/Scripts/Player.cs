using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    //!! TODO: delegate many numbers to GM, or to idea-specific stats
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

    //!! TODO: encapsulation; possibly readonly with special change logic
    private List<Prophet> prophets = new List<Prophet>();
    public List<Prophet> Prophets
    {
        get
        {
            return prophets;
        }
        set
        {
            prophets = value;
        }
    }

    //!! TODO: just store full idea data
    private Color color;
    public Color Color
    {
        get { return color; }
        set
        {
            color = value;
        }
    }

    //!! TODO: encapsulate
    public Sprite TileSprite { get; set; }

    public string Name { get; set; }
    //!! TODO: encapsulate
    public Text ScoreDisplay { get; set; }

    private float score;
    public float Score
    {
        get { return score; }
        set
        {
            score = value;
            //!! TODO: this does not belong anywhere near here. Move win score to game options
            if (ScoreDisplay != null)
                ScoreDisplay.text = "Score: " + value;
            if (score > 10)
            {
                GameManager.Instance.Winner(this);
            }
        }
    }
}
