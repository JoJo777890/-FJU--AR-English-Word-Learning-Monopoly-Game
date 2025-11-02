using ARMonopoly_V5___Full_Scale_V2.Board;
using ARMonopoly_V5___Full_Scale_V2.Data;
using ARMonopoly_V5___Full_Scale_V2.Economy;
using ARMonopoly_V5___Full_Scale_V2.Gameplay;
using ARMonopoly_V5___Full_Scale_V2.Property; // ADDED THIS
using ARMonopoly_V5___Full_Scale_V2.Spelling;
using System.Collections.Generic; // ADDED THIS
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
        public SpellingQuestionDatabase SpellingDB;

        [Header("Scene References")]
        public ARCrosswordScanner CrosswordScanner;

        // Services
        public GameStateMachine StateMachine { get; private set; }
        public Bank Bank { get; private set; }
        public InvestmentService Investments { get; private set; }

        // **FIXED: Central list to solve dependency bug**
        [Header("Runtime Lists")]
        public List<PropertyTag> AllSceneProperties = new List<PropertyTag>();

        // Runtime State
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
            Investments = new InvestmentService(Bank, Config);
        }

        private void Start()
        {
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

