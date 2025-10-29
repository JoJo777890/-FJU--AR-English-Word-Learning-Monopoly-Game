using System.Collections.Generic;
using ARMonopoly_V5___Full_Scale_V2.Data;
using ARMonopoly_V5___Full_Scale_V2.Economy;
using ARMonopoly_V5___Full_Scale_V2.Player;
using UnityEngine;

namespace ARMonopoly_V5___Full_Scale_V2.Core
{
    /// <summary>
    /// Central singleton for the game.
    /// Holds references to core systems and SOs.
    /// Manages game state and services.
    /// </summary>
    [DefaultExecutionOrder(-200)] // Ensure this runs first
    public class AppGame : MonoBehaviour
    {
        public static AppGame Instance { get; private set; }

        [Header("Data Assets")]
        public GameConfig Config;
        public PropertyDatabase PropertyDB;
        public RentCalculator RentCalculator;

        // Core Systems
        public GameStateMachine StateMachine { get; private set; }
        public Bank Bank { get; private set; }

        // Runtime Lookups
        private Dictionary<int, Wallet> _playerWallets = new Dictionary<int, Wallet>();
        private Dictionary<string, int> _propertyOwners = new Dictionary<string, int>(); // PropertyID -> PlayerID

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            
            // Initialize Core Systems
            StateMachine = new GameStateMachine();
            Bank = new Bank();
        }

        void Start()
        {
            StateMachine.SetState(GameState.WaitingForPlayers);
            // In a real game, you'd wait for players to register
            // For this demo, we find them all at the start
            RegisterAllPlayers();
            StateMachine.SetState(GameState.WaitingForRoll);
        }

        void RegisterAllPlayers()
        {
            var players = FindObjectsOfType<PlayerTag>();
            foreach (var player in players)
            {
                var wallet = player.GetComponent<Wallet>();
                if (wallet != null)
                {
                    _playerWallets[player.PlayerID] = wallet;
                    wallet.SetBalance(Config.StartingMoney);
                }
            }
        }

        // --- Public API for Services ---

        public Wallet GetWallet(int playerID)
        {
            _playerWallets.TryGetValue(playerID, out var wallet);
            return wallet;
        }

        public int GetPropertyOwner(string propertyID)
        {
            _propertyOwners.TryGetValue(propertyID, out var ownerID);
            return ownerID == 0 ? -1 : ownerID; // Return -1 for unowned (assuming PlayerID > 0)
        }

        public void SetPropertyOwner(string propertyID, int playerID)
        {
            _propertyOwners[propertyID] = playerID;
        }
    }
}
