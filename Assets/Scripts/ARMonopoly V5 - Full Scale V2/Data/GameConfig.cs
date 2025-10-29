using UnityEngine;

namespace ARMonopoly_V5___Full_Scale_V2.Data
{
    [CreateAssetMenu(menuName = "AR Monopoly/Game Config")]
    public class GameConfig : ScriptableObject
    {
        [Header("Rules")]
        public int StartingMoney = 1500;
        public int GoMoney = 200;
        public int JailFine = 50;
        public int MaxJailTurns = 3;
        public bool AllowDoublesRule = true;

        [Header("AR Proximity")]
        public float TriggerDistance = 0.07f;
        public float DwellSeconds = 0.35f;
        public float ScaleUpFactor = 1.3f;
        public string ContentSuffix = "Plane";
    }
}