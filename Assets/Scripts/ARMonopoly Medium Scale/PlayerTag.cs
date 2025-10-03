using UnityEngine;

namespace ARMonopoly_Medium_Scale 
{
    public class PlayerTag : MonoBehaviour {
        public int playerId = 0;
        public string playerName = "Player 1";
        public int startingMoney = 1500;
        [HideInInspector] public int money;

        private void Awake() { money = startingMoney; }
    }
}