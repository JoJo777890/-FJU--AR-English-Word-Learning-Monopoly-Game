// Assets/Scripts/Simple/PlayerTag.cs
using UnityEngine;

namespace ARMonopoly.Simple
{
    public class PlayerTag : MonoBehaviour
    {
        [Header("Player")]
        public int playerId = 0;
        public string playerName = "Player 1";
        public int startingMoney = 1500;

        [HideInInspector] public int money; // runtime
        private void Awake() => money = startingMoney;
    }
}