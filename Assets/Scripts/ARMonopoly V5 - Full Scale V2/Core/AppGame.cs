using ARMonopoly_V5___Full_Scale_V2.Board;
using ARMonopoly_V5___Full_Scale_V2.Data;
using ARMonopoly_V5___Full_Scale_V2.Economy;
using UnityEngine;

namespace ARMonopoly_V5___Full_Scale_V2.Core
{
    /// <summary>
    /// Central singleton that holds references to core systems and data assets.
    /// Attached to the [GameSystems] GameObject.
    /// </summary>
    [DefaultExecutionOrder(-100)]
    public class AppGame : MonoBehaviour
    {
        public static AppGame Instance { get; private set; }

        [Header("Data Assets")]
        public GameConfig Config;
        public PropertyDatabase PropertyDB;
        public BoardDefinition Board;
        public RentCalculator RentCalculator;

        [Header("Core Systems")]
        public Bank Bank { get; private set; }
        public GameStateMachine StateMachine { get; private set; }

        [Header("Runtime State")]
        [Tooltip("The destination the current player is expected to move to.")]
        public string ExpectedDestinationPropertyID;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            // Initialize core systems
            Bank = new Bank();
            StateMachine = new GameStateMachine();
            
            // PlayerTag.cs will register itself with the Bank.
            // This approach will be more decoupled.
            
            if (Config == null)
            {
                Debug.LogError("AppGame: GameConfig is not assigned!");
            }
            
            // [[Old Code]]: 
            // // Register players with the bank
            // if (Config != null)
            // {
            //     foreach(var player in FindObjectsOfType<Scene.PlayerTag>())
            //     {
            //         Bank.RegisterPlayer(player.PlayerID, Config.StartingMoney);
            //     }
            // }
            // else
            // {
            //     Debug.LogError("AppGame: GameConfig is not assigned!");
            // }

            Debug.Log("[AppGame] Initialized.");
        }

        private void Start()
        {
            // Move to the first state (will be handled by TurnController)
            // StateMachine.SetState(GameState.PlayerTurn);
        }
    }
}