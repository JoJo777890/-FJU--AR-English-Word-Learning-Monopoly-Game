using ARMonopoly_V4___Full_Scale_V1.Core;
using UnityEngine;

namespace ARMonopoly_V4___Full_Scale_V1
{
    public class DebugMessage : MonoBehaviour
    {
        private void OnEnable()
        {
            GameEvents.GameStarted += OnGameStartedMessage;
        }

        private void OnDisable()
        {
            GameEvents.GameStarted -= OnGameStartedMessage;
        }

        void OnGameStartedMessage()
        {
            Debug.Log($"(Debug Message) - Game Started");
        }
    }
}
