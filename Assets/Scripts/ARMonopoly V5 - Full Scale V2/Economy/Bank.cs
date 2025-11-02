using System.Collections.Generic;
using ARMonopoly_V5___Full_Scale_V2.Core;
using ARMonopoly_V5___Full_Scale_V2.Data;
using UnityEngine;

namespace ARMonopoly_V5___Full_Scale_V2.Economy
{
    /// <summary>
    /// A service class (not a MonoBehaviour) that handles all money and property ownership.
    /// Instantiated and held by AppGame.
    /// </summary>
    public class Bank
    {
        // Tracks money for each player
        private Dictionary<int, Wallet> _wallets = new Dictionary<int, Wallet>();
        
        // Tracks ownership: <PropertyID, Owner_PlayerID>
        private Dictionary<string, int> _propertyOwners = new Dictionary<string, int>();

        public void RegisterPlayer(int playerID, int startingMoney)
        {
            if (!_wallets.ContainsKey(playerID))
            {
                _wallets[playerID] = new Wallet(playerID, startingMoney);
                GameEvents.RaiseMoneyChanged(playerID, startingMoney);
            }
        }

        public Wallet GetWallet(int playerID)
        {
            _wallets.TryGetValue(playerID, out Wallet wallet);
            return wallet;
        }

        public int GetPropertyOwner(string propertyID)
        {
            _propertyOwners.TryGetValue(propertyID, out int ownerID);
            return ownerID != 0 ? ownerID : -1; // Return -1 for unowned
        }
        
        public bool BuyProperty(int playerID, PropertyDef property)
        {
            if (property == null) return false;
            
            int owner = GetPropertyOwner(property.PropertyID);
            if (owner != -1) return false; // Already owned

            Wallet payerWallet = GetWallet(playerID);
            if (payerWallet == null) return false;

            if (payerWallet.GetBalance() >= property.Price)
            {
                payerWallet.Remove(property.Price);
                _propertyOwners[property.PropertyID] = playerID;
                Debug.Log($"[Bank] Player {playerID} bought {property.PropertyID}");
                return true;
            }
            
            return false; // Not enough money
        }

        public bool TransferRent(int payerID, int ownerID, int amount)
        {
            Wallet payerWallet = GetWallet(payerID);
            Wallet ownerWallet = GetWallet(ownerID);

            if (payerWallet == null || ownerWallet == null) return false;
            
            // Handle insufficient funds if necessary
            if (payerWallet.GetBalance() < amount)
            {
                // (Future) Bankruptcy logic
            }
            
            payerWallet.Remove(amount);
            ownerWallet.Add(amount);
            return true;
        }
    }
}
