using System;
using UnityEngine;

namespace ARMonopoly_V4___Full_Scale_V1.Core
{
    public class AppGame : MonoBehaviour
    {
        public static AppGame Instance { get; private set; }

        private void Awake()
        {
            // This is the "private set" in action.
            // The App class assigns itself to the static Instance.
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); // Optional: keeps it alive between scenes
            }
            else
            {
                Destroy(gameObject); // Destroy any duplicates
            }
        }

        private void Start()
        {
            StartGame();
        }

        private void StartGame()
        {
            GameStateMachine.Instance.SetState(GameState.Playing);

            // Kick off the first turn.
            GameEvents.RaiseGameStarted();

            // If you have a TurnController registered in ServiceLocator, you can fetch and StartTurn() here.
            // var turn = _services.Get<TurnController>(); turn.StartFirstTurn();
        }
    }
}

