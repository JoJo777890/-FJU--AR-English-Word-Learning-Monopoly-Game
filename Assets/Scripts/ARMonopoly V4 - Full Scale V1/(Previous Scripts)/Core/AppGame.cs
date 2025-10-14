using ARMonopoly_V4___Full_Scale_V1._Previous_Scripts_.Data;
using ARMonopoly_V4___Full_Scale_V1._Previous_Scripts_.Players;
using ARMonopoly_V4___Full_Scale_V1._Previous_Scripts_.Services;
using UnityEngine;

namespace ARMonopoly_V4___Full_Scale_V1._Previous_Scripts_.Core
{
    public class AppGame : MonoBehaviour
    {
        [Header("Data")]
        public GameConfig gameConfig;
        public PropertyDatabase propertyDb;

        // Services
        private EconomyService _economy;
        private OwnershipService _ownership;

        public int CurrentPlayerId { get; private set; } = 0;

        private void Awake()
        {
            _economy = new EconomyService();
            _ownership = new OwnershipService();
        }

        private void Start()
        {
            // seed all players in scene
            foreach (var p in FindObjectsOfType<PlayerTag>())
            {
                int start = (p.startingMoney > 0) ? p.startingMoney : gameConfig.defaultStartMoney;
                _economy.RegisterPlayer(
                    p.playerId, 
                    start
                    );
            }
        }

        public T GetService<T>() where T : class
        {
            if (typeof(T) == typeof(EconomyService))  
                return _economy as T;
            if (typeof(T) == typeof(OwnershipService)) 
                return _ownership as T;
            
            return null;
        }

        public void SetCurrentPlayer(int pid)
        {
            CurrentPlayerId = pid;
        }
    }
}