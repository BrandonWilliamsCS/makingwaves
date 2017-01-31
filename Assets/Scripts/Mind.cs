using System.Collections.Generic;
using UnityEngine;

namespace MakingWaves.Model
{
    public abstract class Mind : MonoBehaviour
    {
        #region Configurable
        // TODO: better option than storing defaults here?
        [SerializeField]
        protected float topHealth = 10f;

        /// <summary>
        /// Gets the health threshold.
        /// This is how many influence points it takes before we can emit influence
        /// </summary>
        /// <value>The health threshold.</value>
        [SerializeField]
        protected float evangelismThreshold = 7f;

        /// <summary>
        /// Gets the health threshold.
        /// This is how many influence points it takes before we can emit influence
        /// </summary>
        /// <value>The health threshold.</value>
        [SerializeField]
        protected float ownershipThreshold = 3f;

        [SerializeField]
        protected float conversionStrength = 1f;
        #endregion

        #region Presentation
        // TODO: Mind seems like a model class; find a better place for this presentation info.
        //   Consider owning a mind instead of being one. 
        private ParticleSystem _influenceEmitter;
        private ParticleSystem InfluenceEmitter
        {
            get
            {
                if (_influenceEmitter == null)
                {
                    _influenceEmitter = GetComponentInChildren<ParticleSystem>();
                }
                return _influenceEmitter;
            }
        }

        protected void Emit()
        {
            if (CanEvangelize && InfluenceEmitter != null)
            {
                var main = InfluenceEmitter.main;
                main.startColor = Owner.Idea.color;
                InfluenceEmitter.Play();
            }
        }
        #endregion

        #region Influence Mechanics
        private Player leader;
        public Player Owner
        {
            get { return currentHealth > 0 ? leader : null; }
            set { leader = value; }
        }
        
        protected float currentHealth = 0f;
        private float calculatedHealth = 0f;

        // TODO: looooong method. break into sub-functions, and ideally prep for configuration or mechanic change.
        public virtual void AcceptInfluence(IEnumerable<Mind> influencers)
        {
            // TODO: better way to disable, especially considering that Mind won't always be a GameObject
            if (!gameObject.activeInHierarchy) return;

            if (currentHealth > evangelismThreshold)
            {
                Emit();
                // TODO: we should consider moving this to the end of the conversion round.
                Owner.Score += Owner.ConversionRateMultiplier * conversionStrength;
            }

            //Calculate neighbor evangelism and control
            // TODO: consider how/if we want to prevent prophets from affecting themselves.
            calculatedHealth = currentHealth;
            float neutralConversionRateMultiplier = .2f;
            if (calculatedHealth > 0f)
            {
                foreach (var influencer in influencers)
                {
                    if (influencer.CanEvangelize)
                    {
                        var ownerShipMultiplier = leader == influencer.leader ? 1 : -1;
                        calculatedHealth += influencer.leader.ConversionRateMultiplier * influencer.conversionStrength * ownerShipMultiplier;
                    }
                    else
                    {
                        if (!influencer.IsOwned)
                        {
                            calculatedHealth -= neutralConversionRateMultiplier * influencer.conversionStrength;
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
                    if (influencer.CanEvangelize)
                    {
                        if (influenceScore.ContainsKey(influencer.leader))
                        {
                            influenceScore[influencer.leader] += influencer.leader.ConversionRateMultiplier * influencer.conversionStrength;
                        }
                        else
                        {
                            influenceScore[influencer.leader] = influencer.leader.ConversionRateMultiplier * influencer.conversionStrength;
                        }
                    }
                    else
                    {
                        if (!influencer.IsOwned)
                        {
                            neutralInfluence += neutralConversionRateMultiplier * influencer.conversionStrength;
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
                    //Leader.nodes.remove(this);
                    leader = effectiveLeader;
                    //Leader.nodes.add(this);
                }
            }
        }

        public bool IsOwned { get { return currentHealth >= ownershipThreshold; } }

        public bool CanEvangelize { get { return currentHealth >= evangelismThreshold; } }

        public virtual void ApplyInfluence()
        {
            if (calculatedHealth > topHealth)
            {
                calculatedHealth = topHealth;
            }
            else if (calculatedHealth < 0)
            {
                calculatedHealth = 0;
            }
            currentHealth = calculatedHealth;
        }

        public virtual void SetOwner(Player player)
        {
            Owner = player;
            currentHealth = topHealth;
        }
        #endregion
    }
}
