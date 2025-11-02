// In folder: ARMonopoly V5 - Full Scale V2/Core/
using ARMonopoly_V5___Full_Scale_V2.Board;
using ARMonopoly_V5___Full_Scale_V2.Data;
using ARMonopoly_V5___Full_Scale_V2.Economy;
using UnityEngine;

namespace ARMonopoly_V5___Full_Scale_V2.Core
{
    [DefaultExecutionOrder(-100)]
    public class AppGame : MonoBehaviour
    {
        public static AppGame Instance { get; private set; }

        [Header("Data Assets")]
        public GameConfig Config;
        public PropertyDatabase PropertyDB;
        public BoardDefinition Board;
        public RentCalculator RentCalculator;
        public SpellingQuestionDatabase SpellingDB; // New

        [Header("Core Systems")]
        public Bank Bank { get; private set; }
        public GameStateMachine StateMachine { get; private set; }

        [Header("Runtime State")]
        public string ExpectedDestinationPropertyID;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            Bank = new Bank();
            StateMachine = new GameStateMachine();
            
            if (Config == null) Debug.LogError("AppGame: GameConfig is not assigned!");
            if (SpellingDB == null) Debug.LogError("AppGame: SpellingDB is not assigned!");


            Debug.Log("[AppGame] Initialized.");
        }
    }
}