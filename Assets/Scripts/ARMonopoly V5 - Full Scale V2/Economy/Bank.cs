using System.Collections.Generic;
using ARMonopoly_V5___Full_Scale_V2.Core;
using ARMonopoly_V5___Full_Scale_V2.Data;
using UnityEngine;

namespace ARMonopoly_V5___Full_Scale_V2.Economy
{
    /// <summary>
    /// Manages all player wallets and property ownership.
    /// Acts as the central ledger for all financial transactions.
    /// </summary>
    public class Bank
    {
        // Tracks money for each player
        private Dictionary<int, Wallet> _wallets = new Dictionary<int, Wallet>();
        
        // Tracks ownership: <PropertyID, Owner_PlayerID>
        private Dictionary<string, int> _propertyOwners = new Dictionary<string, int>();

        /// <summary>
        /// Registers a new player, creating their wallet with starting money.
        /// Fires OnMoneyChanged to update UI.
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
        /// Attempts to purchase an unowned property for a player.
        /// </summary>
        /// <returns>True if the purchase was successful, false otherwise.</returns>
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
        /// Transfers a specified rent amount between two players.
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
        
        // --- Investment Logic ---
        
        /// <summary>
        /// Takes an investment from a player during a spelling challenge.
        /// </summary>
        public void TakeInvestment(int investorID, int amount)
        {
            GetWallet(investorID)?.Remove(amount);
            GameEvents.RaiseLogMessage($"[Bank] P{investorID} invested ${amount}.");
        }

        /// <summary>
        ///         /// Rewards an investor with their original investment plus a reward.
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