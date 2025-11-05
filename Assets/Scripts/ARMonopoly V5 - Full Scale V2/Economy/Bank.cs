// In folder: ARMonopoly V5 - Full Scale V2/Economy/
using System.Collections.Generic;
using ARMonopoly_V5___Full_Scale_V2.Core;
using ARMonopoly_V5___Full_Scale_V2.Data;
using UnityEngine;

namespace ARMonopoly_V5___Full_Scale_V2.Economy
{
    public class Bank
    {
        private Dictionary<int, Wallet> _wallets = new Dictionary<int, Wallet>();
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
                // Log is handled by RuleEngine
                return true;
            }
            
            return false; // Not enough money
        }

        public bool TransferRent(int payerID, int ownerID, int amount)
        {
            Wallet payerWallet = GetWallet(payerID);
            Wallet ownerWallet = GetWallet(ownerID);

            if (payerWallet == null || ownerWallet == null) return false;
            
            payerWallet.Remove(amount);
            ownerWallet.Add(amount);
            
            GameEvents.RaiseLogMessage($"Rent: ${amount} transferred from P{payerID} to P{ownerID}.");
            return true;
        }
        
        // --- New Investment Methods ---
        public void TakeInvestment(int investorID, int amount)
        {
            GetWallet(investorID)?.Remove(amount);
            GameEvents.RaiseLogMessage($"[Bank] P{investorID} invested ${amount}.");
        }

        public void RewardInvestment(int investorID, int originalAmount)
        {
            // Return original investment + reward
            int totalReturn = originalAmount + originalAmount; 
            GetWallet(investorID)?.Add(totalReturn);
            GameEvents.RaiseLogMessage($"[Bank] Rewarded P{investorID} with ${totalReturn}.");
        }
    }
}