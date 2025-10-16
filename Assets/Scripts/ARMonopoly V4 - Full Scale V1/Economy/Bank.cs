using UnityEngine;

namespace ARMonopoly_V4___Full_Scale_V1.Economy
{
    public class Bank : MonoBehaviour
    {
        public static Bank Instance { get; private set; }

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
        
        public void Deposit(Wallet wallet, int amount)
        {
            
        }
    
        public void Withdraw(Wallet wallet, int amount)
        {
            
        }
    
        public void TransferMoney(Wallet from, Wallet to, int amount)
        {
            
        }
    }
}

