using UnityEngine;

namespace ARMonopoly_V5___Full_Scale_V2.Data
{
    /// <summary>
    /// ScriptableObject holding global game configuration settings.
    /// Create one from 'Assets > Create > ARMonopoly_V5___Full_Scale_V2 > Game Config'.
    /// </summary>
    [CreateAssetMenu(menuName = "ARMonopoly_V5___Full_Scale_V2/Game Config")]
    public class GameConfig : ScriptableObject
    {
        [Header("Money")]
        /// <summary>The amount of money a player starts the game with.</summary>
        public int StartingMoney = 1500;
        /// <summary>The amount of money received for passing 'Go'.</summary>
        public int PassGoMoney = 200;

        [Header("AR Proximity")]
        /// <summary>The distance (in meters) to trigger proximity events.</summary>
        public float TriggerDistance = 0.1f;
        /// <summary>The name suffix of the child GameObject to scale (e.g., 'Model', 'Plane').</summary>
        [Tooltip("The name suffix of the child GameObject to scale (e.g., 'Model', 'Plane')")]
        public string ContentSuffix = "Plane";
        /// <summary>The multiplier for scaling up visuals on proximity.</summary>
        public float ScaleUpFactor = 1.3f;
    }
}