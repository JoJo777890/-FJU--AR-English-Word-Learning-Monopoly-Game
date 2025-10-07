using ARMonopoly_Medium_Scale.Gameplay;
using UnityEngine;

namespace ARMonopoly_Medium_Scale.UI
{
    public class RollButton : MonoBehaviour
    {
        TurnController _turn;

        void Start()
        {
            _turn = FindObjectOfType<TurnController>();
        }

        public void OnRollClicked()
        {
            _turn?.RollDice();
        }
    }
}