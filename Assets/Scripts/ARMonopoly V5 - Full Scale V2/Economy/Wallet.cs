using ARMonopoly_V5___Full_Scale_V2.Core;

namespace ARMonopoly_V5___Full_Scale_V2.Economy
{
    /// <summary>
    /// A service class (not a MonoBehaviour) that holds a single player's money.
    /// </summary>
    public class Wallet
    {
        public int PlayerID { get; private set; }
        public int Money { get; private set; }

        public Wallet(int playerID, int startingMoney)
        {
            PlayerID = playerID;
            Money = startingMoney;
        }

        /// <summary>
        /// Gets the current balance of the wallet.
        /// </summary>
        public int GetBalance() => Money;

        /// <summary>
        /// Adds money to the wallet and raises the OnMoneyChanged event.
        /// </summary>
        public bool Add(int amount)
        {
            if (amount < 0) return false;
            Money += amount;
            GameEvents.RaiseMoneyChanged(PlayerID, Money);
            return true;
        }

        /// <summary>
        /// Removes money from the wallet and raises the OnMoneyChanged event.
        /// </summary>
        public bool Remove(int amount)
        {
            if (amount < 0) return false;

            // TODO: Implement proper bankruptcy logic instead of allowing a negative balance.
            // For now, we allow the balance to go negative.
            Money -= amount;
            GameEvents.RaiseMoneyChanged(PlayerID, Money);
            return true;
        }
    }
}