using ARMonopoly_V5___Full_Scale_V2.Core;
using ARMonopoly_V5___Full_Scale_V2.Player;
using UnityEngine;

namespace ARMonopoly_V5___Full_Scale_V2.Economy
{
    /// <summary>
    /// Attached to each PlayerTag GameObject. Holds their money.
    /// </summary>
    public class Wallet : MonoBehaviour
    {
        private int _balance;
        private PlayerTag _player;

        void Awake()
        {
            _player = GetComponent<PlayerTag>();
            if (_player == null)
            {
                Debug.LogError("Wallet component requires a PlayerTag component on the same GameObject.");
            }
        }

        public int GetBalance() => _balance;

        public void SetBalance(int newBalance)
        {
            _balance = newBalance;
            GameEvents.RaiseMoneyChanged(_player.PlayerID, _balance);
        }

        public bool Add(int amount)
        {
            if (amount < 0) return false;
            SetBalance(_balance + amount);
            return true;
        }

        public bool Remove(int amount)
        {
            if (amount < 0 || _balance < amount) return false;
            SetBalance(_balance - amount);
            return true;
        }
    }
}