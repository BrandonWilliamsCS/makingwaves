using System.Collections.Generic;
using UnityEngine;

namespace MakingWaves.Model
{
    public abstract class Unit : MonoBehaviour
    {
        public Mind Mind { get; protected set; }

        protected virtual void Awake()
        {
            // TODO: possibly get special values and pass them in here
            Mind = new Mind();
        }

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
            // TODO: something feels off here. Maybe we should assume the Emitter exists.
            if (InfluenceEmitter != null)
            {
                var main = InfluenceEmitter.main;
                main.startColor = Mind.Owner.Idea.color;
                InfluenceEmitter.Play();
            }
        }
        #endregion

        public virtual void SetOwner(Player player)
        {
            Mind.SetOwner(player);
        }

        public virtual void AcceptInfluence(IEnumerable<Mind> influencers)
        {
            // TODO: better way to disable
            if (!gameObject.activeInHierarchy) return;

            if (Mind.CanInfluence)
            {
                Emit();
            }

            Mind.AcceptInfluence(influencers);
        }

        public virtual void ApplyInfluence()
        {
            Mind.ApplyInfluence();
        }
    }
}
