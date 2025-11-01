using ARMonopoly_V5___Full_Scale_V2.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace ARMonopoly_V5___Full_Scale_V2.UI
{
    /// <summary>
    /// A simple helper script to find the TurnController and call OnRollClicked.
    /// Attach this to your "Roll" button.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class RollButton : MonoBehaviour
    {
        private TurnController _turnController;
        private Button _button;

        void Start()
        {
            // Find the TurnController in the scene
            _turnController = FindObjectOfType<TurnController>();
            if (_turnController == null)
            {
                Debug.LogError("RollButton: No TurnController found in scene!");
                return;
            }

            // Find the Button component on this GameObject
            _button = GetComponent<Button>();
            
            // Add OnClick listener
            _button.onClick.AddListener(OnClick);
        }

        public void OnClick()
        {
            if (_turnController != null)
            {
                _turnController.OnRollClicked();
            }
        }
    }
}