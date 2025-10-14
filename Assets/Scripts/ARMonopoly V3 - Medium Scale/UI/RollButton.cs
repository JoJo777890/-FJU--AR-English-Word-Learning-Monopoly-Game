using ARMonopoly_V3___Medium_Scale.Gameplay;
using UnityEngine;

namespace ARMonopoly_V3___Medium_Scale.UI
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