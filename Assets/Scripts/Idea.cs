using System;
using UnityEngine;

namespace MakingWaves.Model
{
    [Serializable]
    public class Idea
    {
        private static readonly float DEFAULT_BASE_INFLUENCE_MULTIPLIER = 1f;

        public string ideaName;

        #region Mechanics
        public float baseInfluenceMultiplier = DEFAULT_BASE_INFLUENCE_MULTIPLIER;
        #endregion

        #region Presentation
        public Color color;
        public Sprite prophetSprite;
        public Sprite ownedNodeSprite;
        #endregion
    }
}
