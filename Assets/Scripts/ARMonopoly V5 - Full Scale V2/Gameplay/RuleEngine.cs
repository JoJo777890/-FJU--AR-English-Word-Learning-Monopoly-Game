// In folder: ARMonopoly V5 - Full Scale V2/Gameplay/
using System.Collections.Generic;
using ARMonopoly_V5___Full_Scale_V2.Board;
using ARMonopoly_V5___Full_Scale_V2.Core;
using ARMonopoly_V5___Full_Scale_V2.Data;
using ARMonopoly_V5___Full_Scale_V2.Economy;
using UnityEngine;

namespace ARMonopoly_V5___Full_Scale_V2.Gameplay
{
    public class RuleEngine : MonoBehaviour
    {
        private Bank _bank;
        private BoardDefinition _board;
        private RentCalculator _rentCalculator;
        private GameStateMachine _stateMachine;
        private TurnController _turnController;
        private SpellingQuestionDatabase _spellingDB;

        // Runtime state for the current spelling question
        private SpellingQuestion _currentQuestion;
        private string _propertyInQuestionID;
        private List<int> _investors = new List<int>();


        private void Awake()
        {
            _bank = AppGame.Instance.Bank;
            _board = AppGame.Instance.Board;
            _rentCalculator = AppGame.Instance.RentCalculator;
            _stateMachine = AppGame.Instance.StateMachine;
            _turnController = GetComponent<TurnController>();
            _spellingDB = AppGame.Instance.SpellingDB;
        }

        private void OnEnable()
        {
            GameEvents.OnProximityEnter += HandleProximityEnter;
            GameEvents.OnBuyRequest += HandleBuyRequest;
            GameEvents.OnPlayerPassedGo += HandlePassedGo;
            GameEvents.OnPlayerInvest += HandleInvestment;
            GameEvents.OnSpellingAnswer += HandleSpellingAnswer;
        }

        private void OnDisable()
        {
            GameEvents.OnProximityEnter -= HandleProximityEnter;
            GameEvents.OnBuyRequest -= HandleBuyRequest;
            GameEvents.OnPlayerPassedGo -= HandlePassedGo;
            GameEvents.OnPlayerInvest -= HandleInvestment;
            GameEvents.OnSpellingAnswer -= HandleSpellingAnswer;
        }

        private void HandleProximityEnter(ProximityPayload payload)
        {
            if (_stateMachine.CurrentState != GameState.AwaitingPlayerMove) return;
            if (payload.PlayerID != _turnController.CurrentPlayerID) return;
            if (payload.PropertyID != AppGame.Instance.ExpectedDestinationPropertyID) return;

            Debug.Log($"Player {payload.PlayerID} correctly landed on {payload.PropertyID}. Starting Spelling Round.");
            _stateMachine.SetState(GameState.AwaitingSpellingAnswer);

            _propertyInQuestionID = payload.PropertyID;
            _currentQuestion = _spellingDB.GetRandomQuestion();
            _investors.Clear();

            if (_currentQuestion != null)
            {
                GameEvents.RaiseSpellingQuestion(new SpellingQuestionPayload
                {
                    PlayerID = payload.PlayerID,
                    Question = _currentQuestion,
                    PropertyID = _propertyInQuestionID
                });
            }
            else
            {
                Debug.LogError("No spelling questions found in the database! Ending turn.");
                _turnController.EndTurn();
            }
        }
        
        private void HandleInvestment(InvestmentPayload payload)
        {
            if (_stateMachine.CurrentState != GameState.AwaitingSpellingAnswer) return;

            PropertyDef prop = AppGame.Instance.PropertyDB.GetProperty(_propertyInQuestionID);
            int investmentAmount = prop.Price / 2;

            if (_bank.GetWallet(payload.InvestorID).GetBalance() >= investmentAmount)
            {
                _bank.TakeInvestment(payload.InvestorID, investmentAmount);
                _investors.Add(payload.InvestorID);
                Debug.Log($"Player {payload.InvestorID} invested in Player {payload.TargetPlayerID}.");
            }
        }
        
        private void HandleSpellingAnswer(SpellingAnswerPayload payload)
        {
            if (_stateMachine.CurrentState != GameState.AwaitingSpellingAnswer) return;
            _stateMachine.SetState(GameState.ResolvingSpelling);

            bool isCorrect = payload.Answer.ToUpper() == _currentQuestion.CorrectAnswer.ToUpper();
            int currentPlayerID = payload.PlayerID;
            PropertyDef prop = AppGame.Instance.PropertyDB.GetProperty(_propertyInQuestionID);

            if (isCorrect)
            {
                Debug.Log($"Player {currentPlayerID} answered correctly!");
                // Handle rewards for investors
                foreach (var investorID in _investors)
                {
                    _bank.RewardInvestment(investorID, prop.Price / 2);
                }

                int ownerID = _bank.GetPropertyOwner(prop.PropertyID);
                if (ownerID == -1) // Unowned property
                {
                    // Allow the player to buy
                    GameEvents.RaiseBuyPrompt(new BuyPayload
                    {
                        PlayerID = currentPlayerID, 
                        PropertyID = prop.PropertyID,
                        PropertyName = prop.DisplayName, 
                        Price = prop.Price
                    });
                }
                else // Owned property
                {
                    Debug.Log($"Player {currentPlayerID} answered correctly and waives rent.");
                    _turnController.EndTurn();
                }
            }
            else
            {
                Debug.Log($"Player {currentPlayerID} answered incorrectly.");
                // Investors lose their money (Bank already has it)
                Debug.Log($"Investors lost their investment.");

                int ownerID = _bank.GetPropertyOwner(prop.PropertyID);
                 if (ownerID == -1) // Unowned
                {
                    _bank.GetWallet(currentPlayerID).Remove(_currentQuestion.FineAmount);
                    Debug.Log($"Player {currentPlayerID} was fined ${_currentQuestion.FineAmount}");
                    _turnController.EndTurn();
                }
                else if(ownerID != currentPlayerID) // Owned by someone else
                {
                    int rentAmount = _rentCalculator.CalculateRent(prop);
                    _bank.TransferRent(currentPlayerID, ownerID, rentAmount);
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
        
        private void HandleBuyRequest(string propertyID)
        {
            if (_stateMachine.CurrentState != GameState.ResolvingSpelling && _stateMachine.CurrentState != GameState.ResolvingSpace) return;
            
            int playerID = _turnController.CurrentPlayerID;
            PropertyDef propToBuy = AppGame.Instance.PropertyDB.GetProperty(propertyID);

            if (_bank.BuyProperty(playerID, propToBuy))
            {
                GameEvents.RaisePropertyBought(new PropertyPayload
                {
                    PlayerID = playerID, 
                    PropertyID = propToBuy.PropertyID,
                    PropertyName = propToBuy.DisplayName, 
                    Price = propToBuy.Price
                });
            }
            _turnController.EndTurn();
        }

        private void HandlePassedGo(int playerID)
        {
            int passGoMoney = AppGame.Instance.Config.PassGoMoney;
            Wallet wallet = _bank.GetWallet(playerID);
            if (wallet != null)
            {
                wallet.Add(passGoMoney);
                Debug.Log($"Player {playerID} passed Go, credited ${passGoMoney}");
            }
        }
    }
}