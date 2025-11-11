using ARMonopoly_V5___Full_Scale_V2.Board;
using ARMonopoly_V5___Full_Scale_V2.Data;
using ARMonopoly_V5___Full_Scale_V2.Economy;
using UnityEngine;

namespace ARMonopoly_V5___Full_Scale_V2.Core
{
    /// <summary>
    /// Central singleton (Monobehaviour) that holds references to core systems
    /// and ScriptableObject data assets. Attached to the [GameSystems] GameObject.
    /// </summary>
    [DefaultExecutionOrder(-100)]
    public class AppGame : MonoBehaviour
    {
        /// <summary>
        /// Static singleton instance for easy global access.
        /// </summary>
        public static AppGame Instance { get; private set; }

        [Header("Data Assets")]
        // (Tip): Global game settings (starting money, etc.)
        public GameConfig Config;
        // (Tip): Database of all PropertyDef assets for easy lookup.
        public PropertyDatabase PropertyDB;
        // (Tip): ScriptableObject defining the board's logical layout and order.
        public BoardDefinition Board;
        // (Tip): ScriptableObject holding the rent calculation logic.
        public RentCalculator RentCalculator;
        // (Tip): Database of all SpellingQuestion assets.
        public SpellingQuestionDatabase SpellingDB;

        [Header("Core Systems")]
        /// <summary>Service for managing all player wallets and property ownership.</summary>
        public Bank Bank { get; private set; }
        /// <summary>Service for managing the global GameState.</summary>
        public GameStateMachine StateMachine { get; private set; }

        [Header("Runtime State")]
        // (Tip): The destination property ID the current player is expected to move to.
        public string ExpectedDestinationPropertyID;

        void Awake()
        {
            // Enforce singleton pattern
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            // Initialize core non-MonoBehaviour systems
            Bank = new Bank();
            StateMachine = new GameStateMachine();
            
            // Note: PlayerTag.cs instances will register themselves with the Bank
            // during their own Start() phase to ensure AppGame is ready.
            
            if (Config == null) Debug.LogError("AppGame: GameConfig is not assigned!");
            if (SpellingDB == null) Debug.LogError("AppGame: SpellingDB is not assigned!");


            Debug.Log("[AppGame] Initialized.");
        }
    }
}