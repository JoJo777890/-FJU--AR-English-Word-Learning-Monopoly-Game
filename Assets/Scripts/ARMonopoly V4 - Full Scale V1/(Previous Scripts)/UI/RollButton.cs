using ARMonopoly_V4___Full_Scale_V1._Previous_Scripts_.Gameplay;
using UnityEngine;
using TurnController = ARMonopoly_V4___Full_Scale_V1.Gameplay.TurnController;

namespace ARMonopoly_V4___Full_Scale_V1._Previous_Scripts_.UI
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