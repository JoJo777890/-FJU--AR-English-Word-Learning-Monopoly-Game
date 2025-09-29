using UnityEngine;

namespace ARMonopoly.Data
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "ARMonopoly/GameConfig")]
    public class GameConfig : ScriptableObject
    {
        public int startMoney = 1500;
        public float pawnMoveSpeed = 1.0f;
    }
}


