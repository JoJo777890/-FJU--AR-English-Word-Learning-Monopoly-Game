using System;
using System.Collections.Generic;
using ARMonopoly_V4___Full_Scale_V1.Data;
using ARMonopoly_V4___Full_Scale_V1.Economy;
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

        // private void Start()
        // {
        //     StartGame();
        // }
        //
        // private void StartGame()
        // {
        //     GameStateMachine.Instance.SetState(GameState.Playing);
        //
        //     // Kick off the first turn.
        //     GameEvents.RaiseGameStarted();
        //
        //     // If you have a TurnController registered in ServiceLocator, you can fetch and StartTurn() here.
        //     // var turn = _services.Get<TurnController>(); turn.StartFirstTurn();
        // }
        
        [SerializeField] private GameState currentState;
        
        // private List<PlayerTag> players;
        // private int currentPlayerIndex = 0;
        //
        // public GameConfig gameConfig;
        //
        // public int CurrentPlayerId { get; private set; } = 0;
        
        private void Start()
        {
            // BankRegisterPlayer();
        }

        public void GameStart()
        {
            UpdateGameState(GameState.GameStart);
        }
            
        // This is the core of the GameManager. It controls the game flow.
        public void UpdateGameState(GameState newState)
        {
            currentState = newState;

            // The switch statement determines what happens when we enter a new state.
            // These are for visualizing the Core Game-State Flow.
            switch (currentState)
            {
                case GameState.GameStart:
                    // ...Write Here
                    GameEvents.RaiseGameStart();
                    PrintCurrentGameState();
                    UpdateGameState(GameState.TurnStart);
                    break;
                
                case GameState.TurnStart:
                    // ...Write Here
                    GameEvents.RaiseTurnStart();
                    PrintCurrentGameState();
                    UpdateGameState(GameState.RollDice);
                    break;
                
                case GameState.RollDice:
                    // ...Write Here
                    GameEvents.RaiseRollDice();
                    PrintCurrentGameState();
                    UpdateGameState(GameState.WaitForPlayerToMoveToken);
                    break;
                
                case GameState.WaitForPlayerToMoveToken:
                    // ...Write Here
                    GameEvents.RaiseWaitForPlayerToMoveToken();
                    PrintCurrentGameState();
                    UpdateGameState(GameState.PlayerMovedToken);
                    break;
                
                case GameState.PlayerMovedToken:
                    // ...Write Here
                    GameEvents.RaiseWaitForPlayerToMoveToken();
                    PrintCurrentGameState();
                    UpdateGameState(GameState.PayRent);
                    break;
                
                case GameState.PayRent:
                    // ...Write Here
                    GameEvents.RaisePayRent();
                    PrintCurrentGameState();
                    UpdateGameState(GameState.BuyProperty);
                    break;
                
                case GameState.BuyProperty:
                    // ...Write Here
                    GameEvents.RaiseBuyProperty();
                    PrintCurrentGameState();
                    UpdateGameState(GameState.TurnEnd);
                    break;
                
                case GameState.TurnEnd:
                    // ...Write Here
                    GameEvents.RaiseTurnEnd();
                    PrintCurrentGameState();
                    UpdateGameState(GameState.GameEnd);
                    break;
                
                case GameState.GameEnd:
                    // ...Write Here
                    GameEvents.RaiseGameEnd();
                    PrintCurrentGameState();
                    break;
                //
                //
                //
                // case GameState.GameSetup:
                //     // Logic to set up the board, initialize players, etc.
                //     Debug.Log("Game Setup: Initializing players and board.");
                //     // After setup is complete, move to the next state.
                //     UpdateGameState(GameState.WaitingForTurnStart);
                //     break;
                //
                // case GameState.WaitingForTurnStart:
                //     Debug.Log("Waiting for turn to start...");
                //     // In a real game, you might wait for a UI button press.
                //     // For now, we'll just start the turn automatically.
                //     // StartNextTurn();
                //     break;
                //
                // case GameState.PlayerTurn:
                //     PlayerTag currentPlayer = players[currentPlayerIndex];
                //     Debug.Log($"It is now {currentPlayer.playerName}'s turn.");
                //     // The game is now waiting for the player to move their physical token.
                //     // The AR system will raise an event when a move is detected.
                //     break;
                //
                // case GameState.WaitingForPlayerAction:
                //     // The player has landed on a property. The game is paused,
                //     // waiting for them to make a choice (e.g., buy/pass via UI).
                //     Debug.Log("Waiting for player to decide on an action...");
                //     break;
                //     
                // case GameState.SpellingChallenge:
                //     // A spelling challenge has been triggered.
                //     // The game waits for the AR SpellingValidator to raise a success/fail event.
                //     Debug.Log("Spelling challenge initiated!");
                //     break;

                // case GameState.TurnEnd:
                //     Debug.Log("Ending the current turn.");
                //     // Move to the next player.
                //     currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
                //     UpdateGameState(GameState.WaitingForTurnStart);
                //     break;
            }
        }

        public GameState GetCurrentGameState()
        {
            return currentState;
        }

        public void PrintCurrentGameState()
        {
            Debug.Log($"(Current_State: {GetCurrentGameState()})...)");
        }
        
        // public void SetCurrentPlayer(int pid)
        // {
        //     CurrentPlayerId = pid;
        // }

        // private void BankRegisterPlayer()
        // {
        //     foreach (var p in FindObjectsOfType<PlayerTag>())
        //     {
        //         int start = (p.startingMoney > 0) ? p.startingMoney : gameConfig.defaultStartMoney;
        //         Bank.Instance.RegisterPlayer(
        //             p.playerId,
        //             start
        //         );
        //     }
        // }
    }
}

