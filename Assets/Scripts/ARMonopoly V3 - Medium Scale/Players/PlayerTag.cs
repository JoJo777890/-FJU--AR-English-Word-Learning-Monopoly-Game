using UnityEngine;

namespace ARMonopoly_V3___Medium_Scale.Players 
{
    public class PlayerTag : MonoBehaviour {
        public int playerId = 0;
        public string playerName = "Player 1";
        public int startingMoney = 1500;
        [HideInInspector] public int money;

        private void Awake() 
        {
            money = startingMoney;
        }
    }
}