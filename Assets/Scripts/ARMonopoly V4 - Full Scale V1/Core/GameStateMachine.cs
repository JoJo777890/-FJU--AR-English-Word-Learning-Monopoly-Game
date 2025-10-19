

using UnityEngine;

namespace ARMonopoly_V4___Full_Scale_V1.Core
{
    public class GameStateMachine: MonoBehaviour
    {
        public GameState State { get; private set; }

        // Singleton Class
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
    }

    public enum GameState
    {
        // GameSetup, 
        // WaitingForTurnStart, 
        // PlayerTurn, 
        // WaitingForPlayerAction, 
        // SpellingChallenge, 
        //
        GameSetup,
        GameStarted, 
        TurnStarted,
        RolledDice, 
        WaitForPlayerToMoveToken, 
        PlayerMovedToken,
        PayRent, 
        BuyProperty, 
        TurnEnded, 
        GameEnded
    }
    // Example: 
    // (
    // Boot,
    // Scanning, 
    // Calibrating,
    // BoardReady, 
    // Paused, 
    // Results
    // )
}


