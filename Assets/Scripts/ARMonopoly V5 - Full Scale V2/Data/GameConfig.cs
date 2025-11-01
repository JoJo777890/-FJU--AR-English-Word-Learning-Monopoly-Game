using UnityEngine;

namespace ARMonopoly_V5___Full_Scale_V2.Data
{
    /// <summary>
    /// Holds global game settings.
    /// Create one from 'Assets > Create > AR Monopoly > Game Config'.
    /// </summary>
    [CreateAssetMenu(menuName = "ARMonopoly_V5___Full_Scale_V2/Game Config")]
    public class GameConfig : ScriptableObject
    {
        [Header("Money")]
        public int StartingMoney = 1500;
        public int PassGoMoney = 200;

        [Header("AR Proximity")]
        public float TriggerDistance = 0.1f;
        [Tooltip("The name suffix of the child GameObject to scale (e.g., 'Model', 'Plane')")]
        public string ContentSuffix = "Plane";
        public float ScaleUpFactor = 1.3f;
    }
}