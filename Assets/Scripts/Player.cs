using MakingWaves.Model;
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

    public Idea Idea { get; set; }

    //!! TODO: encapsulation; possibly readonly with special change logic
    [SerializeField]
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
    public Color Color { get { return Idea.color; } }

    //!! TODO: encapsulate
    public Sprite TileSprite { get { return Idea.ownedNodeSprite; } }

    public string Name { get { return Idea.ideaName; } }

    //!! TODO: encapsulate
    public Text ScoreDisplay { get; set; }

    public float Score { get; set; }

    public void InitializeProphets(Node startNode)
    {
        foreach (var prophet in Prophets)
        {
            prophet.GetComponentInChildren<SpriteRenderer>().sprite = Idea.prophetSprite;
            prophet.Leader = this;
            prophet.CurrentHealth = prophet.TopHealth;

            if (startNode != null)
            {
                prophet.CurrentNode = startNode;
                prophet.transform.position = startNode.transform.position;
                startNode.SetOwner(this);
            }
            else
            {
                prophet.gameObject.SetActive(false);
            }
        }
    }
}
