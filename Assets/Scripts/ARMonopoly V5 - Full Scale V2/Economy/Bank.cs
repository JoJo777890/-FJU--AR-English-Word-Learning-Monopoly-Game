namespace ARMonopoly_V5___Full_Scale_V2.Economy
{
    /// <summary>
    /// A service class (not a MonoBehaviour) that handles money transfers.
    /// Instantiated and held by AppGame.
    /// </summary>
    public class Bank
    {
        public bool Transfer(Wallet from, Wallet to, int amount)
        {
            if (from == null || to == null || amount < 0) return false;

            if (from.GetBalance() >= amount)
            {
                from.Remove(amount);
                to.Add(amount);
                return true;
            }
            return false;
        }
        
        public bool Pay(Wallet from, int amount)
        {
            if (from == null || amount < 0) return false;
            return from.Remove(amount);
        }
        
        public bool Receive(Wallet to, int amount)
        {
            if (to == null || amount < 0) return false;
            return to.Add(amount);
        }
    }
}