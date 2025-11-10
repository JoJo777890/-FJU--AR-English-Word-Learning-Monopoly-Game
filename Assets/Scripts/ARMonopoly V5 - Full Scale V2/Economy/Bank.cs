using System.Collections.Generic;
using ARMonopoly_V5___Full_Scale_V2.Core;
using ARMonopoly_V5___Full_Scale_V2.Data;
using UnityEngine;

namespace ARMonopoly_V5___Full_Scale_V2.Economy
{
    /// <summary>
    /// A service class (not a MonoBehaviour) that manages all money and property ownership.
    /// Instantiated and held by AppGame.
    /// </summary>
    public class Bank
    {
        // Tracks money for each player
        private Dictionary<int, Wallet> _wallets = new Dictionary<int, Wallet>();
        
        // Tracks ownership: <PropertyID, Owner_PlayerID>
        private Dictionary<string, int> _propertyOwners = new Dictionary<string, int>();

        /// <summary>
        /// Creates a new Wallet for a player and registers them with the bank.
        /// </summary>
        public void RegisterPlayer(int playerID, int startingMoney)
        {
            if (!_wallets.ContainsKey(playerID))
            {
                _wallets[playerID] = new Wallet(playerID, startingMoney);
                GameEvents.RaiseMoneyChanged(playerID, startingMoney);
            }
        }

        /// <summary>
        /// Retrieves the Wallet for a specific player.
        /// </summary>
        public Wallet GetWallet(int playerID)
        {
            _wallets.TryGetValue(playerID, out Wallet wallet);
            return wallet;
        }

        /// <summary>
        /// Gets the PlayerID of the owner of a property.
        /// </summary>
        /// <returns>The owner's PlayerID, or -1 if unowned.</returns>
        public int GetPropertyOwner(string propertyID)
        {
            _propertyOwners.TryGetValue(propertyID, out int ownerID);
            return ownerID != 0 ? ownerID : -1; // Return -1 for unowned
        }
        
        /// <summary>
        /// Attempts to process a property purchase for a player.
        /// </summary>
        /// <returns>True if successful, false otherwise.</returns>
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
                // Log message is handled by RuleEngine
                return true;
            }
            
            return false; // Not enough money
        }

        /// <summary>
        /// Transfers rent from one player's wallet to another.
        /// </summary>
        /// <returns>True if successful.</returns>
        public bool TransferRent(int payerID, int ownerID, int amount)
        {
            Wallet payerWallet = GetWallet(payerID);
            Wallet ownerWallet = GetWallet(ownerID);

            if (payerWallet == null || ownerWallet == null) return false;
            
            // TODO: Handle bankruptcy logic if payerWallet.GetBalance() < amount
            
            payerWallet.Remove(amount);
            ownerWallet.Add(amount);
            
            GameEvents.RaiseLogMessage($"Rent: ${amount} transferred from P{payerID} to P{ownerID}.");
            return true;
        }
        
        /// <summary>
        /// Takes the investment amount from the investor's wallet (held by Bank).
        /// </summary>
        public void TakeInvestment(int investorID, int amount)
        {
            GetWallet(investorID)?.Remove(amount);
            GameEvents.RaiseLogMessage($"[Bank] P{investorID} invested ${amount}.");
        }

        /// <summary>
        /// Rewards an investor by returning their original stake plus a bonus.
        /// </summary>
        public void RewardInvestment(int investorID, int originalAmount)
        {
            // Return original investment + reward (50% + 50% = 100% of property price)
            int totalReturn = originalAmount + originalAmount; 
            GetWallet(investorID)?.Add(totalReturn);
            GameEvents.RaiseLogMessage($"[Bank] Rewarded P{investorID} with ${totalReturn}.");
        }
    }
}