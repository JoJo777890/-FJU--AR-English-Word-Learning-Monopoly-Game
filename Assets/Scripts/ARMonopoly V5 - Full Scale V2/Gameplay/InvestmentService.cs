using System.Collections.Generic;
using ARMonopoly_V5___Full_Scale_V2.Core;
using ARMonopoly_V5___Full_Scale_V2.Data;
using ARMonopoly_V5___Full_Scale_V2.Economy;
using UnityEngine;

namespace ARMonopoly_V5___Full_Scale_V2.Gameplay
{
    /// <summary>
    /// Holds the state of pending investments for the current spelling quiz.
    /// </summary>
    public class PendingInvestment
    {
        public int PlayerID { get; set; }
        public PropertyDef Property { get; set; }
        public int Amount { get; set; }
    }

    /// <summary>
    /// Manages player investment chances and handles payouts.
    /// </summary>
    public class InvestmentService
    {
        private Dictionary<int, int> _investmentChancesLeft = new Dictionary<int, int>();
        private List<PendingInvestment> _pendingInvestments = new List<PendingInvestment>();
        private Bank _bank;
        private GameConfig _config;

        public InvestmentService(Bank bank, GameConfig config)
        {
            _bank = bank;
            _config = config;
        }

        public void RegisterPlayer(int playerID)
        {
            _investmentChancesLeft[playerID] = _config.MaxInvestmentsPerPlayer;
        }

        public int GetChancesLeft(int playerID)
        {
            _investmentChancesLeft.TryGetValue(playerID, out int chances);
            return chances;
        }

        /// <summary>
        /// A player attempts to invest in the current quiz.
        /// </summary>
        public bool TryInvest(int playerID, PropertyDef property)
        {
            if (GetChancesLeft(playerID) <= 0)
            {
                Debug.Log($"Player {playerID} has no investment chances left.");
                return false;
            }

            Wallet wallet = _bank.GetWallet(playerID);
            int investmentCost = Mathf.FloorToInt(property.Price * 0.5f);

            if (wallet.Money >= investmentCost)
            {
                // Take collateral
                wallet.Remove(investmentCost);
                _investmentChancesLeft[playerID]--;

                _pendingInvestments.Add(new PendingInvestment
                {
                    PlayerID = playerID,
                    Property = property,
                    Amount = investmentCost
                });
                
                Debug.Log($"Player {playerID} invested ${investmentCost} in {property.DisplayName}. {_investmentChancesLeft[playerID]} chances left.");
                GameEvents.RaisePlayerInvested(playerID, property.DisplayName, investmentCost);
                return true;
            }
            
            Debug.Log($"Player {playerID} cannot afford to invest ${investmentCost}.");
            return false;
        }

        /// <summary>
        /// The player answered the quiz. Resolve all pending investments.
        /// </summary>
        public void ResolveInvestments(bool correctAnswer)
        {
            if (correctAnswer)
            {
                // CORRECT: Pay back original stake + reward
                foreach (var investment in _pendingInvestments)
                {
                    Wallet investorWallet = _bank.GetWallet(investment.PlayerID);
                    int reward = investment.Amount; // 50%
                    int totalPayout = investment.Amount + reward; // 50% + 50% = 100%
                    
                    investorWallet.Add(totalPayout); // Bank pays them
                    Debug.Log($"Player {investment.PlayerID} gets ${totalPayout} reward for correct investment.");
                    GameEvents.RaiseQuizResult(
                        $"Investor {investment.PlayerID} CORRECT!",
                        $"You get your ${investment.Amount} back, plus a ${reward} reward!");
                }
            }
            else
            {
                // WRONG: Investors lose their money (it's already taken).
                foreach (var investment in _pendingInvestments)
                {
                    Debug.Log($"Player {investment.PlayerID} lost their ${investment.Amount} investment.");
                    GameEvents.RaiseQuizResult(
                        $"Investor {investment.PlayerID} WRONG!",
                        $"You lost your ${investment.Amount} investment.");
                }
            }
            
            _pendingInvestments.Clear();
        }
    }
}
