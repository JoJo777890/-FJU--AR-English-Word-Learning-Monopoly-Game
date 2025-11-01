using System.Collections.Generic;
using ARMonopoly_V5___Full_Scale_V2.Core;
using ARMonopoly_V5___Full_Scale_V2.Gameplay;
// FIXED: Using .Scene namespace
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ARMonopoly_V5___Full_Scale_V2.UI
{
    public class UIManager : MonoBehaviour
    {
        [Header("Turn UI")]
        public TextMeshProUGUI TurnText;
        public TextMeshProUGUI DiceRollText;
        public Button RollButton;

        [Header("Player Money")]
        public List<TextMeshProUGUI> MoneyTexts; // Index 0 = P1, 1 = P2, etc.

        [Header("Move Notification")]
        public GameObject MoveNotificationPanel;
        public TextMeshProUGUI MoveNotificationText;
        
        [Header("Buy Panel")]
        public GameObject BuyPanel;
        public TextMeshProUGUI BuyPromptText;
        public Button BuyButton_Confirm;
        public Button BuyButton_Pass;

        [Header("Spelling Quiz Panel")]
        public GameObject SpellingQuizPanel;
        public TextMeshProUGUI SpellingQuestionText;
        public Button InvestButton;
        public TextMeshProUGUI InvestButtonText;
        public Button CheckAnswerButton;

        [Header("Quiz Result Panel")]
        public GameObject QuizResultPanel;
        public TextMeshProUGUI QuizResultTitle;
        public TextMeshProUGUI QuizResultMessage;
        public Button QuizResultOkButton;
        
        [Header("Log Panel")]
        public TextMeshProUGUI LogText;

        private string _buyPropertyID; // Store ID for buy/pass
        private TurnController _turnController; // FIXED: Cached reference

        private void Awake()
        {
            // FIXED: Get reference once
            _turnController = FindObjectOfType<TurnController>(); 
            
            // Wire up button clicks
            RollButton.onClick.AddListener(() => {
                // The RollButton script will handle this
            });
            
            BuyButton_Confirm.onClick.AddListener(() => {
                GameEvents.RaiseBuyRequest(_buyPropertyID);
                BuyPanel.SetActive(false);
            });
            
            BuyButton_Pass.onClick.AddListener(() => {
                GameEvents.RaisePassRequest(_buyPropertyID);
                BuyPanel.SetActive(false);
            });
            
            // --- NEW QUIZ BUTTONS ---
            InvestButton.onClick.AddListener(() => {
                // Find the next player who ISN'T the current player
                int currentPlayer = _turnController.CurrentPlayerID;
                
                // Simple 2-player toggle. This will need to be smarter for 3+ players.
                int nextPlayer = (currentPlayer == 1) ? 2 : 1; 
                
                GameEvents.RaiseInvestRequest(new InvestRequest { PlayerID = nextPlayer });
            });

            CheckAnswerButton.onClick.AddListener(() => {
                int currentPlayer = _turnController.CurrentPlayerID;
                GameEvents.RaiseCheckAnswerRequest(new CheckAnswerRequest { PlayerID = currentPlayer });
                SpellingQuizPanel.SetActive(false);
            });
            
            QuizResultOkButton.onClick.AddListener(() => {
                QuizResultPanel.SetActive(false);
            });
            
            // Hide all panels at start
            MoveNotificationPanel.SetActive(false);
            BuyPanel.SetActive(false);
            SpellingQuizPanel.SetActive(false);
            QuizResultPanel.SetActive(false);
        }

        private void OnEnable()
        {
            GameEvents.OnStateChanged += HandleStateChanged;
            GameEvents.OnTurnStarted += HandleTurnStarted;
            GameEvents.OnDiceRolled += HandleDiceRolled;
            GameEvents.OnMoveRequired += HandleMoveRequired;
            GameEvents.OnBuyPrompt += HandleBuyPrompt;
            GameEvents.OnMoneyChanged += HandleMoneyChanged;
            GameEvents.OnRentPaid += HandleRentPaid;
            GameEvents.OnPropertyBought += HandlePropertyBought;
            
            // --- NEW QUIZ EVENTS ---
            GameEvents.OnSpellingQuizStarted += HandleSpellingQuizStarted;
            GameEvents.OnPlayerInvested += HandlePlayerInvested;
            GameEvents.OnQuizResult += HandleQuizResult;
        }

        private void OnDisable()
        {
            GameEvents.OnStateChanged -= HandleStateChanged;
            GameEvents.OnTurnStarted -= HandleTurnStarted;
            GameEvents.OnDiceRolled -= HandleDiceRolled;
            GameEvents.OnMoveRequired -= HandleMoveRequired;
            GameEvents.OnBuyPrompt -= HandleBuyPrompt;
            GameEvents.OnMoneyChanged -= HandleMoneyChanged;
            GameEvents.OnRentPaid -= HandleRentPaid;
            GameEvents.OnPropertyBought -= HandlePropertyBought;
            
            // --- NEW QUIZ EVENTS ---
            GameEvents.OnSpellingQuizStarted -= HandleSpellingQuizStarted;
            GameEvents.OnPlayerInvested -= HandlePlayerInvested;
            GameEvents.OnQuizResult -= HandleQuizResult;
        }
        
        private void HandleStateChanged(GameState newState)
        {
            // Only allow rolling in the PlayerTurn state
            RollButton.interactable = (newState == GameState.PlayerTurn);
            
            // Hide panels based on state
            if (newState != GameState.AwaitingPlayerMove)
                MoveNotificationPanel.SetActive(false);
            
            if (newState != GameState.AwaitingSpelling)
                SpellingQuizPanel.SetActive(false);
                
            // BuyPanel is shown by OnBuyPrompt, which happens *after* ResolvingSpelling
            if (newState != GameState.ResolvingSpelling)
                BuyPanel.SetActive(false);
        }

        private void HandleTurnStarted(int playerID)
        {
            TurnText.text = $"Player {playerID}'s Turn";
            DiceRollText.text = "Roll the dice!";
            AddLog($"--- Player {playerID}'s Turn ---");
        }

        private void HandleDiceRolled(int playerID, int roll)
        {
            DiceRollText.text = $"Player {playerID} rolled a {roll}!";
        }

        private void HandleMoveRequired(MovePayload payload)
        {
            MoveNotificationPanel.SetActive(true);
            MoveNotificationText.text = $"Player {payload.PlayerID}, please move your token to: {payload.DestinationName}";
        }

        private void HandleBuyPrompt(BuyPayload payload)
        {
            _buyPropertyID = payload.PropertyID;
            BuyPanel.SetActive(true);
            BuyPromptText.text = $"Player {payload.PlayerID}, buy {payload.PropertyName} for ${payload.Price}?";
        }

        private void HandleMoneyChanged(int playerID, int newBalance)
        {
            int index = playerID - 1; // Assuming P1=index 0, P2=index 1
            if (index >= 0 && index < MoneyTexts.Count)
            {
                MoneyTexts[index].text = $"P{playerID}: ${newBalance}";
            }
        }
        
        // --- NEW HANDLERS ---
        
        private void HandleSpellingQuizStarted(SpellingQuizPayload payload)
        {
            SpellingQuizPanel.SetActive(true);
            SpellingQuestionText.text = payload.Question.QuestionText;
            
            // Update the Invest button
            // This assumes 2 players. A real system would be more complex.
            int currentPlayer = payload.PlayerID;
            int otherPlayer = (currentPlayer == 1) ? 2 : 1;
            int chances = AppGame.Instance.Investments.GetChancesLeft(otherPlayer);
            
            InvestButtonText.text = $"P{otherPlayer}: Invest? ({chances} left)";
            InvestButton.interactable = (chances > 0);
            
            // Only the current player can check the answer
            CheckAnswerButton.gameObject.SetActive(true); 
        }

        private void HandlePlayerInvested(int playerID, string propertyName, int amount)
        {
            AddLog($"Player {playerID} invested ${amount} in {propertyName}!");
            // Update the button text to show they've invested
            InvestButtonText.text = $"P{playerID} Invested!";
            InvestButton.interactable = false;
        }

        private void HandleQuizResult(QuizResultPayload payload)
        {
            QuizResultPanel.SetActive(true);
            QuizResultTitle.text = payload.Title;
            QuizResultMessage.text = payload.Message;
            AddLog($"{payload.Title} {payload.Message}");
        }
        
        // --- LOG HANDLERS ---

        private void HandleRentPaid(RentPayload payload)
        {
            AddLog($"Player {payload.PayerID} paid ${payload.Amount} rent to Player {payload.OwnerID} for {payload.PropertyName}.");
        }

        private void HandlePropertyBought(PropertyPayload payload)
        {
            AddLog($"Player {payload.PlayerID} bought {payload.PropertyName} for ${payload.Price}!");
        }

        private void AddLog(string message)
        {
            if (LogText == null) return;
            LogText.text = message + "\n" + LogText.text;
            if (LogText.text.Length > 1000) // Prune log
            {
                LogText.text = LogText.text.Substring(0, 1000);
            }
        }
    }
}

