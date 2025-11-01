using UnityEngine;

namespace ARMonopoly_V5___Full_Scale_V2.Data
{
    [CreateAssetMenu(menuName = "ARMonopoly_V5___Full_Scale_V2/Game Config")]
    public class GameConfig : ScriptableObject
    {
        [Header("Money")]
        public int StartingMoney = 1500;
        public int PassGoMoney = 200;

        [Header("AR Interaction")]
        public float ProximityTriggerDistance = 0.07f; // meters
        public float ProximityDwellTime = 0.5f;     // seconds
        public float PropertyScaleUpFactor = 1.3f;
        public string PropertyContentSuffix = "Plane";

        [Header("Spelling Game")]
        public int MaxInvestmentsPerPlayer = 4;
        [Tooltip("Fine as a percentage of property price (e.g., 0.1 for 10%)")]
        public float WrongAnswerFinePercent = 0.1f;
    }
}