using UnityEngine;

namespace ARMonopoly_Medium_Scale.Data
{
    [CreateAssetMenu(menuName="ARMonopolyV2-MediumScale/Game Config")]
    public class GameConfig : ScriptableObject
    {
        [Header("Rules")]
        public int defaultStartMoney = 1500;
        public int goMoney = 200;
        public bool useDoublesExtraTurn = true;
        public int jailFine = 50;
        public int maxJailTurns = 3;

        [Header("AR/UX")]
        public float triggerDistance = 0.07f;
        public float dwellSeconds = 0.35f;
        public float scaleUpFactor = 1.3f;
        public string contentSuffix = "Plane";
    }
}