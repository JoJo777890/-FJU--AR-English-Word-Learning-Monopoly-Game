using System.Collections.Generic;
using ARMonopoly_V4___Full_Scale_V1.Core;
using UnityEngine;

namespace ARMonopoly_V4___Full_Scale_V1.Economy
{
    public class Bank : MonoBehaviour
    {
        public static Bank Instance { get; private set; }
        
        private readonly Dictionary<int,int> _money = new();

        private void Awake()
        {
            // This is the "private set" in action.
            // The Bank class assigns itself to the static Instance.
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
        
        public void RegisterPlayer(int pid, int startMoney) 
        {
            if (!_money.ContainsKey(pid)) 
                _money[pid] = startMoney;
            
            GameEvents.RaiseMoneyChanged(new MoneyChanged
            {
                playerId = pid, 
                money = _money[pid]
            });
        }
        
        public int GetMoney(int pid)
        {
            return _money.TryGetValue(pid, out var m) ? m : 0;
        }

        public void Credit(int pid, int amount) 
        {
            _money[pid] = GetMoney(pid) + amount;
            GameEvents.RaiseMoneyChanged(new MoneyChanged
            {
                playerId = pid, 
                money = _money[pid]
            });
        }
        
        public bool Debit(int pid, int amount) 
        {
            int m = GetMoney(pid);
            if (m < amount)
            {
                _money[pid] = m - amount;
            } // allow negative for simplicity
            else
            {
                _money[pid] = m - amount;
            }
            
            GameEvents.RaiseMoneyChanged(new MoneyChanged
            {
                playerId = pid, 
                money = _money[pid]
            });
            
            return true;
        }
        
        public void Transfer(int from, int to, int amount) 
        {
            Debit(from, amount);
            Credit(to, amount);
        }
    }
}

