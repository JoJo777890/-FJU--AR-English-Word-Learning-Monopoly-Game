using ARMonopoly_V5___Full_Scale_V2.Board;
using ARMonopoly_V5___Full_Scale_V2.Data;
using ARMonopoly_V5___Full_Scale_V2.Economy;
using ARMonopoly_V5___Full_Scale_V2.Gameplay;
using ARMonopoly_V5___Full_Scale_V2.Spelling;
using UnityEngine;

namespace ARMonopoly_V5___Full_Scale_V2.Core
{
    /// <summary>
    /// Central singleton that holds references to core systems and data assets.
    /// Attached to the [GameSystems] GameObject.
    /// </summary>
    public class AppGame : MonoBehaviour
    {
        public static AppGame Instance { get; private set; }

        [Header("Data Assets")]
        public GameConfig Config;
        public PropertyDatabase PropertyDB;
        public BoardDefinition Board;
        public RentCalculator RentCalculator;
        public SpellingQuestionDatabase SpellingDB; // NEW

        [Header("Scene References")]
        public ARCrosswordScanner CrosswordScanner; // NEW: Assign this in Inspector

        // Services
        public GameStateMachine StateMachine { get; private set; }
        public Bank Bank { get; private set; }
        public InvestmentService Investments { get; private set; } // NEW

        // Runtime State
        // The destination the current player is expected to move to.
        public string ExpectedDestinationPropertyID { get; set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            
            // Initialize core systems
            StateMachine = new GameStateMachine();
            Bank = new Bank();
            Investments = new InvestmentService(Bank, Config); // Create new service
        }

        private void Start()
        {
            // Find the scanner if not assigned
            if (CrosswordScanner == null)
            {
                CrosswordScanner = FindObjectOfType<ARCrosswordScanner>();
                if (CrosswordScanner == null)
                {
                    Debug.LogError("AppGame: CRITICAL: ARCrosswordScanner not found in scene!");
                }
            }
        }
    }
}

