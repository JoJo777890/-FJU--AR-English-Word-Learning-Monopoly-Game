using System;
using ARMonopoly_V5___Full_Scale_V2.Board;
using ARMonopoly_V5___Full_Scale_V2.Data;
using ARMonopoly_V5___Full_Scale_V2.Economy;
using ARMonopoly_V5___Full_Scale_V2.Gameplay;
using ARMonopoly_V5___Full_Scale_V2.Player;
using ARMonopoly_V5___Full_Scale_V2.Property;
using ARMonopoly_V5___Full_Scale_V2.UI;
using UnityEngine;

namespace ARMonopoly_V5___Full_Scale_V2.Core
{
    /// <summary>
    /// The Composition Root. 
    /// Responsible for creating services and injecting dependencies into all scene objects.
    /// </summary>
    [DefaultExecutionOrder(-1000)] // Ensures this runs before ANYTHING else
    public class AppGame : MonoBehaviour
    {
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

        [Header("Scene References")]
        [Tooltip("Reference to the UIManager in the scene.")]
        public UIManager UIManager;
        [Tooltip("Reference to the TurnController in the scene.")]
        public TurnController TurnController;
        [Tooltip("Reference to the RuleEngine in the scene.")]
        public RuleEngine RuleEngine;
        [Tooltip("Reference to the DiceScanner in the scene.")]
        public DiceScanner DiceScanner;

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
            if (CheckMissingReferences()) return;

            // 1. Create Core Services
            Bank = new Bank();
            StateMachine = new GameStateMachine();

            Debug.Log("[AppGame] Services Created. Injecting Core Components...");

            // 2. Inject into Core Logic Scripts
            // Pass the specific dependencies each script needs.
            TurnController.Construct(StateMachine, Board, DiceScanner, this); // Passing 'this' only for ExpectedDestinationPropertyID access
            RuleEngine.Construct(Bank, Board, RentCalculator, StateMachine, TurnController, SpellingDB, this);
            UIManager.Construct(TurnController);
        }

        void Start()
        {
            Debug.Log("[AppGame] Starting Scene Object Injection...");
            
            // 3. Inject into Properties (Find all in scene)
            var allProps = FindObjectsOfType<PropertyVisuals>(true);
            foreach (var prop in allProps)
            {
                prop.Construct(Config);
            }
            
            // 4. Inject into Players (Find all in scene)
            var allPlayers = FindObjectsOfType<PlayerTag>(true);
            foreach (var player in allPlayers)
            {
                player.Construct(Bank, Config, Board);
            }
            
            var allTriggers = FindObjectsOfType<PlayerTokenTrigger>(true);
            foreach (var trigger in allTriggers)
            {
                trigger.Construct(Config);
            }
            
            Debug.Log("[AppGame] Dependency Injection Complete.");
            
            // 5. Start the Game manually (Triggers OnTurnStarted)
            TurnController.StartGame();
        }

        private bool CheckMissingReferences()
        {
            if (!Config || !PropertyDB || !Board || !RentCalculator || !SpellingDB)
            {
                Debug.LogError("[AppGame] Missing Data Assets!");
                return true;
            }
            if (!UIManager || !TurnController || !RuleEngine || !DiceScanner)
            {
                Debug.LogError("[AppGame] Missing Scene References! Assign them in Inspector.");
                return true;
            }
            return false;
        }
    }
}