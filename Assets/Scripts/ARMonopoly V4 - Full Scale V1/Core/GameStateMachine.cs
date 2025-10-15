

using UnityEngine;

namespace ARMonopoly_V4___Full_Scale_V1.Core
{
    public class GameStateMachine: MonoBehaviour
    {
        private GameState _state;

        public static GameStateMachine Instance { get; private set; }

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
        
        public GameState GetState()
        {
            return _state;
        }

        public void SetState(GameState state)
        {
            _state = state;
        }
    }

    public enum GameState
    {
        Boot,
        Scanning, 
        Calibrating,
        BoardReady, 
        Playing, 
        Paused, 
        Results
    }
}


