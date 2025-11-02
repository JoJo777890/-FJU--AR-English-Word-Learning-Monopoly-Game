using ARMonopoly_V5___Full_Scale_V2.Board;
using ARMonopoly_V5___Full_Scale_V2.Core;
using ARMonopoly_V5___Full_Scale_V2.Data;
using ARMonopoly_V5___Full_Scale_V2.Economy;
using ARMonopoly_V5___Full_Scale_V2.Gameplay;
using ARMonopoly_V5___Full_Scale_V2.Spelling;
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
        private InvestmentService _investments;
        private SpellingQuestionDatabase _spellingDB;
        private ARCrosswordScanner _scanner;
        private GameConfig _config;

        // Runtime state for the current quiz
        private SpellingQuizPayload _currentQuiz;

        private void Awake()
        {
            _bank = AppGame.Instance.Bank;
            _board = AppGame.Instance.Board;
            _rentCalculator = AppGame.Instance.RentCalculator;
            _stateMachine = AppGame.Instance.StateMachine;
            _investments = AppGame.Instance.Investments;
            _spellingDB = AppGame.Instance.SpellingDB;
            _scanner = AppGame.Instance.CrosswordScanner;
            _config = AppGame.Instance.Config;
            _turnController = GetComponent<TurnController>();

            if (_bank == null || _board == null || _rentCalculator == null || _stateMachine == null || 
                _turnController == null || _investments == null || _spellingDB == null || _scanner == null || _config == null)
            {
                Debug.LogError("RuleEngine: Missing one or more critical references from AppGame or GameObject!");
            }
        }

        private void OnEnable()
        {
            GameEvents.OnProximityEnter += HandleProximityEnter;
            GameEvents.OnBuyRequest += HandleBuyRequest;
            GameEvents.OnPassRequest += HandlePassRequest; // Added Pass handler
            GameEvents.OnPlayerPassedGo += HandlePassedGo;
            GameEvents.OnInvestRequest += HandleInvestRequest;
            GameEvents.OnCheckAnswerRequest += HandleCheckAnswerRequest;
        }

        private void OnDisable()
        {
            GameEvents.OnProximityEnter -= HandleProximityEnter;
            GameEvents.OnBuyRequest -= HandleBuyRequest;
            GameEvents.OnPassRequest -= HandlePassRequest; // Added Pass handler
            GameEvents.OnPlayerPassedGo -= HandlePassedGo;
            GameEvents.OnInvestRequest -= HandleInvestRequest;
            GameEvents.OnCheckAnswerRequest -= HandleCheckAnswerRequest;
        }

        private void HandlePassedGo(int playerID)
        {
            int passGoMoney = _config.PassGoMoney;
            Wallet wallet = _bank.GetWallet(playerID);
            if (wallet != null)
            {
                wallet.Add(passGoMoney);
                Debug.Log($"Player {playerID} passed Go, credited ${passGoMoney}");
            }
        }

        private void HandleProximityEnter(ProximityPayload payload)
        {
            // 1. Is the game waiting for this move?
            if (_stateMachine.CurrentState != GameState.AwaitingPlayerMove)
            {
                // **DEBUG LOG ADDED**
                Debug.Log($"[RuleEngine] Ignored proximity: State is {_stateMachine.CurrentState}, not AwaitingPlayerMove.");
                return;
            }

            // 2. Is it the correct player?
            if (payload.PlayerID != _turnController.CurrentPlayerID)
            {
                // **DEBUG LOG ADDED**
                Debug.Log($"[RuleEngine] Ignored proximity: Wrong player. Got {payload.PlayerID}, expected {_turnController.CurrentPlayerID}.");
                return;
            }

            // 3. Is it the correct destination?
            if (payload.PropertyID != AppGame.Instance.ExpectedDestinationPropertyID)
            {
                Debug.Log($"Player {payload.PlayerID} landed on {payload.PropertyID}, but we are waiting for {AppGame.Instance.ExpectedDestinationPropertyID}");
                return;
            }

            // --- SUCCESS ---
            Debug.Log($"Player {payload.PlayerID} correctly landed on {payload.PropertyID}. Resolving space.");
            
            // Change state to show we are resolving
            _stateMachine.SetState(GameState.ResolvingSpace);
            
            PropertyDef landedProp = _board.GetPropertyAt(_board.GetIndexFromID(payload.PropertyID));
            if (landedProp == null)
            {
                Debug.LogError($"Could not find PropertyDef for ID {payload.PropertyID}");
                _turnController.EndTurn(); // Failsafe
                return;
            }
            
            // --- NEW SPELLING LOGIC ---
            
            int ownerID = _bank.GetPropertyOwner(landedProp.PropertyID);
            bool isBuyQuiz = (ownerID == -1);

            // Get a question and start the quiz
            SpellingQuestion question = _spellingDB.GetRandomQuestion();
            if (question == null)
            {
                Debug.LogError("No spelling questions in database! Skipping quiz.");
                ResolveLanding_Failsafe(payload.PlayerID, landedProp, ownerID);
                return;
            }

            _currentQuiz = new SpellingQuizPayload
            {
                PlayerID = payload.PlayerID,
                Property = landedProp,
                Question = question,
                IsBuyQuiz = isBuyQuiz
            };
            
            // Start the quiz
            _stateMachine.SetState(GameState.AwaitingSpelling);
            GameEvents.RaiseSpellingQuizStarted(_currentQuiz);
        }

        /// <summary>
        /// A non-current player clicks the "Invest" button.
        /// </summary>
        private void HandleInvestRequest(InvestRequest payload)
        {
            if (_stateMachine.CurrentState != GameState.AwaitingSpelling) return;

            // Can't invest in your own quiz
            if (payload.PlayerID == _turnController.CurrentPlayerID) return;

            _investments.TryInvest(payload.PlayerID, _currentQuiz.Property);
        }

        /// <summary>
        // The current player clicks the "Check My Answer" button.
        /// </summary>
        private void HandleCheckAnswerRequest(CheckAnswerRequest payload)
        {
            if (_stateMachine.CurrentState != GameState.AwaitingSpelling) return;
            if (payload.PlayerID != _turnController.CurrentPlayerID) return;

            _stateMachine.SetState(GameState.ResolvingSpelling);

            // 1. Scan the board
            var scannedWords = _scanner.GetScannedWords();
            string correctAnswer = _currentQuiz.Question.CorrectAnswer;
            
            bool isCorrect = false;
            foreach (var word in scannedWords)
            {
                if (word == correctAnswer)
                {
                    isCorrect = true;
                    break;
                }
            }

            // 2. Resolve Investments
            _investments.ResolveInvestments(isCorrect);
            
            // 3. Apply Game Rules
            int playerID = _currentQuiz.PlayerID;
            PropertyDef property = _currentQuiz.Property;

            if (isCorrect)
            {
                GameEvents.RaiseQuizResult("Correct!", $"You spelled {correctAnswer}!");
                
                if (_currentQuiz.IsBuyQuiz)
                {
                    // Player answered correctly. NOW they can buy.
                    // **LOGIC FIX:** We set state to ResolvingSpace so buttons work
                    _stateMachine.SetState(GameState.ResolvingSpace);
                    GameEvents.RaiseBuyPrompt(new BuyPayload
                    {
                        PlayerID = playerID,
                        PropertyID = property.PropertyID,
                        PropertyName = property.DisplayName,
                        Price = property.Price
                    });
                    // Turn ends after Buy/Pass request
                }
                else
                {
                    // Player answered correctly. Rent is waived.
                    Debug.Log($"Player {playerID} spelled correctly. Rent waived.");
                    GameEvents.RaiseQuizResult($"Player {playerID} spelled correctly!", "Rent is waived!");
                    _turnController.EndTurn();
                }
            }
            else
            {
                GameEvents.RaiseQuizResult("Wrong!", $"The correct answer was {correctAnswer}.");

                if (_currentQuiz.IsBuyQuiz)
                {
                    // Player answered wrongly. Fine them and end turn.
                    int fine = Mathf.FloorToInt(property.Price * _config.WrongAnswerFinePercent);
                    _bank.GetWallet(playerID)?.Remove(fine);
                    Debug.Log($"Player {playerID} spelled wrong. Fined ${fine}.");
                    GameEvents.RaiseQuizResult($"Player {playerID} spelled wrong!", $"You are fined ${fine}.");
                    _turnController.EndTurn();
                }
                else
                {
                    // Player answered wrongly. Pay full rent.
                    Debug.Log($"Player {playerID} spelled wrong. Paying full rent.");
                    int ownerID = _bank.GetPropertyOwner(property.PropertyID);
                    int rentAmount = _rentCalculator.CalculateRent(property);
                    _bank.TransferRent(playerID, ownerID, rentAmount);
                    
                    GameEvents.RaiseRentPaid(new RentPayload
                    {
                        PayerID = playerID,
                        OwnerID = ownerID,
                        PropertyID = property.PropertyID,
                        PropertyName = property.DisplayName,
                        Amount = rentAmount
                    });
                    _turnController.EndTurn();
                }
            }
        }

        /// <summary>
        /// Player clicked "Buy" on the Buy Panel.
        /// </summary>
        private void HandleBuyRequest(string propertyID)
        {
            // **LOGIC FIX:** This must check for ResolvingSpace
            if (_stateMachine.CurrentState != GameState.ResolvingSpace) return;
            
            int playerID = _turnController.CurrentPlayerID;
            PropertyDef propToBuy = _board.GetPropertyAt(_board.GetIndexFromID(propertyID));
            
            bool success = _bank.BuyProperty(playerID, propToBuy);
            if (success)
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

        /// <summary>
        /// Player clicked "Pass" on the Buy Panel.
        /// </summary>
        private void HandlePassRequest(string propertyID)
        {
            // **LOGIC FIX:** This must check for ResolvingSpace
            if (_stateMachine.CurrentState != GameState.ResolvingSpace) return;

            Debug.Log($"Player {_turnController.CurrentPlayerID} passed on buying {propertyID}.");
            _turnController.EndTurn();
        }
        
        /// <summary>
        /// Failsafe logic if spelling DB fails.
        /// </summary>
        private void ResolveLanding_Failsafe(int playerID, PropertyDef landedProp, int ownerID)
        {
            if (ownerID == -1)
            {
                GameEvents.RaiseBuyPrompt(new BuyPayload
                {
                    PlayerID = playerID,
                    PropertyID = landedProp.PropertyID,
                    PropertyName = landedProp.DisplayName,
                    Price = landedProp.Price
                });
                // Set state to ResolvingSpace so Buy/Pass buttons work
                _stateMachine.SetState(GameState.ResolvingSpace);
            }
            else if (ownerID != playerID)
            {
                int rentAmount = _rentCalculator.CalculateRent(landedProp);
                _bank.TransferRent(playerID, ownerID, rentAmount);
                GameEvents.RaiseRentPaid(new RentPayload
                {
                    PayerID = playerID,
                    OwnerID = ownerID,
                    PropertyID = landedProp.PropertyID,
                    PropertyName = landedProp.DisplayName,
                    Amount = rentAmount
                });
                _turnController.EndTurn();
            }
            else
            {
                _turnController.EndTurn();
            }
        }
    }
}

