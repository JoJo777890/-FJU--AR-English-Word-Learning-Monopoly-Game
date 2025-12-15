using System.Collections.Generic;
using ARMonopoly_V5___Full_Scale_V2.Board;
using ARMonopoly_V5___Full_Scale_V2.Core;
using ARMonopoly_V5___Full_Scale_V2.Data;
using ARMonopoly_V5___Full_Scale_V2.Economy;
using UnityEngine;

namespace ARMonopoly_V5___Full_Scale_V2.Gameplay
{
    /// <summary>
    /// Main game logic service. Listens for events (like landing) and applies game rules.
    /// Attached to the [GameSystems] GameObject.
    /// </summary>
    public class RuleEngine : MonoBehaviour
    {
        // Core system references
        private Bank _bank;
        private BoardDefinition _board;
        private RentCalculator _rentCalculator;
        private GameStateMachine _stateMachine;
        private TurnController _turnController;
        private SpellingQuestionDatabase _spellingDB;
        private AppGame _appGameContext;

        // For the current spelling question
        private SpellingQuestion _currentQuestion;
        private string _propertyInQuestionID;
        private List<int> _investors = new List<int>();

        /// <summary>
        /// Injection Method.
        /// </summary>
        public void Construct(Bank bank, BoardDefinition board, RentCalculator rentCalc, GameStateMachine stateMachine, TurnController turnController, SpellingQuestionDatabase spellingDB, AppGame appGame)
        {
            _bank = bank;
            _board = board;
            _rentCalculator = rentCalc;
            _stateMachine = stateMachine;
            _turnController = turnController;
            _spellingDB = spellingDB;
            _appGameContext = appGame;
        }

        /// <summary>
        /// Subscribes to all relevant game events.
        /// </summary>
        private void OnEnable()
        {
            GameEvents.OnProximityEnter += HandleProximityEnter;
            GameEvents.OnBuyRequest += HandleBuyRequest;
            GameEvents.OnPlayerPassedGo += HandlePassedGo;
            GameEvents.OnPlayerInvest += HandleInvestment;
            GameEvents.OnSpellingAnswer += HandleSpellingAnswer;
        }

        /// <summary>
        /// Unsubscribes from all events to prevent memory leaks.
        /// </summary>
        private void OnDisable()
        {
            GameEvents.OnProximityEnter -= HandleProximityEnter;
            GameEvents.OnBuyRequest -= HandleBuyRequest;
            GameEvents.OnPlayerPassedGo -= HandlePassedGo;
            GameEvents.OnPlayerInvest -= HandleInvestment;
            GameEvents.OnSpellingAnswer -= HandleSpellingAnswer;
        }

        /// <summary>
        /// Handles the OnProximityEnter event. This is the "verification" step.
        /// It checks if the player landed on the *correct* property.
        /// </summary>
        private void HandleProximityEnter(ProximityPayload payload)
        {
            // 1. Is the game waiting for this move?
            if (_stateMachine == null || _stateMachine.CurrentState != GameState.AwaitingPlayerMove) return;
            
            // 2. Is it the correct player?
            if (payload.PlayerID != _turnController.CurrentPlayerID) return;
            
            // 3. Is it the correct destination?
            if (payload.PropertyID != _appGameContext.ExpectedDestinationPropertyID) return; // Player landed on the wrong property, keep waiting.

            // --- Proximity Verified ---
            Debug.Log($"Player {payload.PlayerID} correctly landed on {payload.PropertyID}.");
            _stateMachine.SetState(GameState.AwaitingSpellingAnswer);

            // --- Start Spelling Challenge ---
            _propertyInQuestionID = payload.PropertyID;
            _currentQuestion = _spellingDB.GetRandomQuestion();
            _investors.Clear();

            if (_currentQuestion != null)
            {
                GameEvents.RaiseLogMessage($"Spelling challenge for P{payload.PlayerID} on {_propertyInQuestionID}!");
                GameEvents.RaiseSpellingQuestion(new SpellingQuestionPayload
                {
                    PlayerID = payload.PlayerID,
                    Question = _currentQuestion,
                    PropertyID = _propertyInQuestionID
                });
            }
            else
            {
                _turnController.EndTurn();
            }
        }

        /// <summary>
        /// Handles the OnPlayerInvest event, triggered by the UI.
        /// </summary>
        private void HandleInvestment(InvestmentPayload payload)
        {
            if (_stateMachine.CurrentState != GameState.AwaitingSpellingAnswer) return;

            PropertyDef prop = _appGameContext.PropertyDB.GetProperty(_propertyInQuestionID);
            int investmentAmount = prop.Price / 2;

            if (_bank.GetWallet(payload.InvestorID).GetBalance() >= investmentAmount)
            {
                _bank.TakeInvestment(payload.InvestorID, investmentAmount);
                _investors.Add(payload.InvestorID);
                GameEvents.RaiseLogMessage($"Player {payload.InvestorID} invested in Player {payload.TargetPlayerID}.");
            }
            else
            {
                GameEvents.RaiseLogMessage($"Player {payload.InvestorID} cannot afford to invest.");
            }
        }

        /// <summary>
        /// Handles the OnSpellingAnswer event, resolves the challenge, and applies rules.
        /// </summary>
        private void HandleSpellingAnswer(SpellingAnswerPayload payload)
        {
            if (_stateMachine.CurrentState != GameState.AwaitingSpellingAnswer) return;
            _stateMachine.SetState(GameState.ResolvingSpelling);

            bool isCorrect = payload.Answer.ToUpper() == _currentQuestion.CorrectAnswer.ToUpper();
            int currentPlayerID = payload.PlayerID;
            PropertyDef prop = _appGameContext.PropertyDB.GetProperty(_propertyInQuestionID);

            if (isCorrect)
            {
                GameEvents.RaiseLogMessage($"Player {currentPlayerID} answered CORRECTLY! ({payload.Answer.ToUpper()})");
                
                foreach (var investorID in _investors)
                {
                    _bank.RewardInvestment(investorID, prop.Price / 2);
                }

                int ownerID = _bank.GetPropertyOwner(prop.PropertyID);
                if (ownerID == -1) // Unowned
                {
                    // Player answered correctly, now show the buy prompt
                    GameEvents.RaiseLogMessage("Player can now buy the property.");
                    GameEvents.RaiseBuyPrompt(new BuyPayload
                    {
                        PlayerID = currentPlayerID, 
                        PropertyID = prop.PropertyID,
                        PropertyName = prop.DisplayName, 
                        Price = prop.Price
                    });
                    // Turn ends when player Buys or Passes via HandleBuyRequest or UIManager.OnBuyPass
                }
                else // Owned (by self or other)
                {
                    GameEvents.RaiseLogMessage($"Player {currentPlayerID} waives rent.");
                    _turnController.EndTurn();
                }
            }
            else // Answer was incorrect
            {
                GameEvents.RaiseLogMessage($"Player {currentPlayerID} answered INCORRECTLY.");
                
                if(_investors.Count > 0)
                    GameEvents.RaiseLogMessage("Investors have lost their investment.");

                int ownerID = _bank.GetPropertyOwner(prop.PropertyID);
                 if (ownerID == -1) // Unowned
                {
                    _bank.GetWallet(currentPlayerID).Remove(_currentQuestion.FineAmount);
                    GameEvents.RaiseLogMessage($"Player {currentPlayerID} is fined ${_currentQuestion.FineAmount}.");
                    _turnController.EndTurn();
                }
                else if(ownerID != currentPlayerID) // Owned by someone else
                {
                    GameEvents.RaiseLogMessage($"Player {currentPlayerID} must pay rent.");
                    int rentAmount = _rentCalculator.CalculateRent(prop);
                    _bank.TransferRent(currentPlayerID, ownerID, rentAmount);
                    // Bank's TransferRent method raises its own log message
                    GameEvents.RaiseRentPaid(new RentPayload
                    {
                        PayerID = currentPlayerID, 
                        OwnerID = ownerID, 
                        PropertyID = prop.PropertyID,
                        PropertyName = prop.DisplayName, 
                        Amount = rentAmount
                    });
                     _turnController.EndTurn();
                }
                else // Landed on their own property
                {
                    _turnController.EndTurn();
                }
            }
        }

        /// <summary>
        /// Handles the OnBuyRequest event, triggered by the UI.
        /// </summary>
        private void HandleBuyRequest(string propertyID)
        {
            // A buy request can come after a spelling challenge (ResolvingSpelling)
            // or if we modify the rules for a direct buy (ResolvingSpace)
            if (_stateMachine.CurrentState != GameState.ResolvingSpelling && _stateMachine.CurrentState != GameState.ResolvingSpace) return;
            
            int playerID = _turnController.CurrentPlayerID;
            PropertyDef propToBuy = _appGameContext.PropertyDB.GetProperty(propertyID);

            if (_bank.BuyProperty(playerID, propToBuy))
            {
                GameEvents.RaiseLogMessage($"Player {playerID} bought {propToBuy.DisplayName} for ${propToBuy.Price}.");
                GameEvents.RaisePropertyBought(new PropertyPayload
                {
                    PlayerID = playerID, 
                    PropertyID = propToBuy.PropertyID,
                    PropertyName = propToBuy.DisplayName, 
                    Price = propToBuy.Price
                });
            }
            else
            {
                GameEvents.RaiseLogMessage($"Player {playerID} failed to buy {propToBuy.DisplayName}.");
            }

            // Whether buy succeeded or failed, the action is resolved. End the turn.
            _turnController.EndTurn();
        }

        /// <summary>
        /// Handles the OnPlayerPassedGo event.
        /// </summary>
        private void HandlePassedGo(int playerID)
        {
            int passGoMoney = _appGameContext.Config.PassGoMoney;
            Wallet wallet = _bank.GetWallet(playerID);
            if (wallet != null)
            {
                wallet.Add(passGoMoney);
                GameEvents.RaiseLogMessage($"Player {playerID} passed Go, credited ${passGoMoney}.");
            }
        }
    }
}