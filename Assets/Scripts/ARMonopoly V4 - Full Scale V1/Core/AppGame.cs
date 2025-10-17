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
        
        [SerializeField]
        private GameState _currentState = GameState.Boot;

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
        
        public void UpdateGameState(GameState newState)
        {
            _currentState = newState;

            // The switch statement determines what happens when we enter a new state.
            switch (_currentState)
            {
                case GameState.Boot:
                    // Logic to set up the board, initialize players, etc.
                    Debug.Log("Game Setup: Initializing players and board.");
                    // After setup is complete, move to the next state.
                    UpdateGameState(GameState.WaitingForTurnStart);
                    break;

                case GameState.WaitingForTurnStart:
                    Debug.Log("Waiting for turn to start...");
                    // In a real game, you might wait for a UI button press.
                    // For now, we'll just start the turn automatically.
                    StartNextTurn();
                    break;

                case GameState.PlayerTurn:
                    Player currentPlayer = players[currentPlayerIndex];
                    Debug.Log($"It is now {currentPlayer.Name}'s turn.");
                    // The game is now waiting for the player to move their physical token.
                    // The AR system will raise an event when a move is detected.
                    break;

                case GameState.WaitingForPlayerAction:
                    // The player has landed on a property. The game is paused,
                    // waiting for them to make a choice (e.g., buy/pass via UI).
                    Debug.Log("Waiting for player to decide on an action...");
                    break;
                    
                case GameState.SpellingChallenge:
                    // A spelling challenge has been triggered.
                    // The game waits for the AR SpellingValidator to raise a success/fail event.
                    Debug.Log("Spelling challenge initiated!");
                    break;

                case GameState.TurnEnd:
                    Debug.Log("Ending the current turn.");
                    // Move to the next player.
                    currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
                    UpdateGameState(GameState.WaitingForTurnStart);
                    break;
            }
        }
        
        private void StartNextTurn()
        {
            Player currentPlayer = players[currentPlayerIndex];
            GameEvents.OnTurnStart?.Invoke(currentPlayer); // Raise event for UI to update
            UpdateGameState(GameState.PlayerTurn);
        }
    }
}

