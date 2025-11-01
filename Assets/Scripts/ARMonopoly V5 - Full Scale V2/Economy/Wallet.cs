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

        public int GetBalance() => Money;

        public bool Add(int amount)
        {
            if (amount < 0) return false;
            Money += amount;
            GameEvents.RaiseMoneyChanged(PlayerID, Money);
            return true;
        }

        public bool Remove(int amount)
        {
            if (amount < 0) return false;
            if (Money < amount)
            {
                // Not enough money, but we'll allow it for now
                // (or return false;)
                Money -= amount;
                GameEvents.RaiseMoneyChanged(PlayerID, Money);
                return true; // Or false, depending on game rules
            }

            Money -= amount;
            GameEvents.RaiseMoneyChanged(PlayerID, Money);
            return true;
        }
    }
}