using MakingWaves.Model;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Idea Idea { get; set; }

    public string Name { get { return Idea.ideaName; } }

    public float InfluenceRateMultiplier { get { return Idea.baseInfluenceMultiplier; } }

    public float Score { get; set; }

    #region Prophet tracking
    private List<Prophet> ownedProphets = new List<Prophet>();

    public void InitializeProphets(Node startNode)
    {
        if (startNode != null)
        {
            startNode.SetOwner(this);
        }
        foreach (var prophet in ownedProphets)
        {
            prophet.GetComponentInChildren<SpriteRenderer>().sprite = Idea.prophetSprite;
            prophet.SetOwner(this);

            if (startNode != null)
            {
                prophet.CurrentNode = startNode;
                prophet.transform.position = startNode.transform.position;
            }
            else
            {
                prophet.gameObject.SetActive(false);
            }
        }
    }

    public IList<Prophet> GetMovableProphets()
    {
        // TODO: work out what is best here.
        return ownedProphets;
    }

    public void UpdateProphetOwnership(Prophet prophet, bool owned)
    {
        if (!owned)
        {
            ownedProphets.Remove(prophet);
        }
        else if (!ownedProphets.Contains(prophet))
        {
            ownedProphets.Add(prophet);
        }
    }
    #endregion
}
