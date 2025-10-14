using UnityEngine;

namespace ARMonopoly_V4___Full_Scale_V1._Previous_Scripts_.Players 
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