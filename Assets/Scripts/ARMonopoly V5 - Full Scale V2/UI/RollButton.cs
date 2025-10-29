using ARMonopoly_V5___Full_Scale_V2.Gameplay;
using UnityEngine;

namespace ARMonopoly_V5___Full_Scale_V2.UI
{
    /// <summary>
    /// A simple helper to hook a UI button to the TurnController.
    /// </summary>
    public class RollButton : MonoBehaviour
    {
        private TurnController _turnController;

        void Start()
        {
            _turnController = FindObjectOfType<TurnController>();
            if (_turnController == null)
            {
                Debug.LogError("RollButton requires a TurnController in the scene.");
            }
        }

        public void OnClick()
        {
            _turnController?.RollDice();
        }
    }
}