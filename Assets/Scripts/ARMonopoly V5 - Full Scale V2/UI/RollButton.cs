using ARMonopoly_V5___Full_Scale_V2.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace ARMonopoly_V5___Full_Scale_V2.UI
{
    /// <summary>
    /// A simple helper script to hook the UI Button to the TurnController.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class RollButton : MonoBehaviour
    {
        private TurnController _turnController;

        private void Start()
        {
            // Find the TurnController in the scene
            _turnController = FindObjectOfType<TurnController>();
            if (_turnController == null)
            {
                Debug.LogError("RollButton: Could not find TurnController!");
                return;
            }

            // Hook up the OnClick event
            GetComponent<Button>().onClick.AddListener(OnRollClicked);
        }

        private void OnRollClicked()
        {
            _turnController.OnRollClicked();
        }
    }
}