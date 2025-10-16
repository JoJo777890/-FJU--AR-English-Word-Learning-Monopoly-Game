using UnityEngine;

namespace ARMonopoly_V4___Full_Scale_V1.Economy
{
    public class Wallet : MonoBehaviour
    {
        private int _balance;

        public int Balance()
        {
            return _balance;
        }

        public void Credit(int amount)
        {
            _balance += amount;
        }

        public void Debit(int amount)
        {
            _balance -= amount;
        }
    }
}
