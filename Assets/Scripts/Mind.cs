using System;
using System.Collections.Generic;

namespace MakingWaves.Model
{
    public class Mind
    {
        // TODO: better option than storing defaults here?
        private float topHealth = 10f;
        private float evangelismThreshold = 7f;
        private float ownershipThreshold = 3f;
        private float baseInfluenceMultiplier = 1f;

        public float CurrentHealth { get; set; }
        private float calculatedHealth = 0f;

        public bool IsOwned { get { return CurrentHealth >= ownershipThreshold; } }

        public float PartOwned { get { return Math.Min(CurrentHealth / ownershipThreshold, 1); } }

        public bool CanInfluence { get { return CurrentHealth >= evangelismThreshold; } }

        public float InfluenceStrength { get { return IsOwned ? baseInfluenceMultiplier * Leader.InfluenceRateMultiplier : 0; } }

        public Player Leader { get; set; }
        public Player Owner { get { return IsOwned ? Leader : null; } }

        // TODO: looooong method. break into sub-functions, and ideally prep for configuration or mechanic change.
        public void AcceptInfluence(IEnumerable<Mind> influencers)
        {
            //Calculate neighbor evangelism and control
            // TODO: consider how/if we want to prevent prophets from affecting themselves.
            calculatedHealth = CurrentHealth;
            float neutralInfluenceMultiplier = .2f;
            if (calculatedHealth > 0f)
            {
                foreach (var influencer in influencers)
                {
                    if (influencer.CanInfluence)
                    {
                        var ownerShipMultiplier = Leader == influencer.Leader ? 1 : -1;
                        calculatedHealth += influencer.Leader.InfluenceRateMultiplier * influencer.baseInfluenceMultiplier * ownerShipMultiplier;
                    }
                    else
                    {
                        if (!influencer.IsOwned)
                        {
                            calculatedHealth -= neutralInfluenceMultiplier * influencer.baseInfluenceMultiplier;
                        }
                    }
                }
            }
            else
            {
                IDictionary<Player, float> influenceScore = new Dictionary<Player, float>();
                float neutralInfluence = 0;
                foreach (var influencer in influencers)
                {
                    if (influencer.CanInfluence)
                    {
                        if (influenceScore.ContainsKey(influencer.Leader))
                        {
                            influenceScore[influencer.Leader] += influencer.Leader.InfluenceRateMultiplier * influencer.baseInfluenceMultiplier;
                        }
                        else
                        {
                            influenceScore[influencer.Leader] = influencer.Leader.InfluenceRateMultiplier * influencer.baseInfluenceMultiplier;
                        }
                    }
                    else
                    {
                        if (!influencer.IsOwned)
                        {
                            neutralInfluence += neutralInfluenceMultiplier * influencer.baseInfluenceMultiplier;
                        }
                    }
                }
                Player effectiveLeader = null;
                foreach (var scorePair in influenceScore)
                {
                    if (scorePair.Value > calculatedHealth)
                    {
                        effectiveLeader = scorePair.Key;
                        calculatedHealth = scorePair.Value;
                    }
                }
                foreach (var scorePair in influenceScore)
                {
                    if (scorePair.Key != effectiveLeader)
                    {
                        calculatedHealth -= scorePair.Value;
                    }
                }
                calculatedHealth -= neutralInfluence;
                if (calculatedHealth > 0)
                {
                    Leader = effectiveLeader;
                }
            }
        }

        public void ApplyInfluence()
        {
            if (calculatedHealth > topHealth)
            {
                calculatedHealth = topHealth;
            }
            else if (calculatedHealth < 0)
            {
                calculatedHealth = 0;
            }
            CurrentHealth = calculatedHealth;
        }

        public void SetOwner(Player player)
        {
            Leader = player;
            CurrentHealth = topHealth;
        }
    }
}
