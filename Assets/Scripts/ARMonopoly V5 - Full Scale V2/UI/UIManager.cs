using System.Collections.Generic;
using ARMonopoly_V5___Full_Scale_V2.Core;
using ARMonopoly_V5___Full_Scale_V2.Gameplay;
// FIXED: Using .Player namespace to match user's file
using ARMonopoly_V5___Full_Scale_V2.Player;
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
        private TurnController _turnController;

        private void Awake()
        {
            _turnController = FindObjectOfType<TurnController>();
            
            // Wire up button clicks
            // **FIXED: Removed the empty RollButton listener.**
            // The RollButton.cs script handles its own click event.
            
            BuyButton_Confirm.onClick.AddListener(() => {
                GameEvents.RaiseBuyRequest(_buyPropertyID);
                BuyPanel.SetActive(false);
            });
            
            BuyButton_Pass.onClick.AddListener(() => {
                GameEvents.RaisePassRequest(_buyPropertyID);
                BuyPanel.SetActive(false);
            });
            
            InvestButton.onClick.AddListener(() => {
                int currentPlayer = _turnController.CurrentPlayerID;
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

        // **FIXED: ADDED START() TO FIX RACE CONDITION**
        private void Start()
        {
            // Manually sync UI to the current game state when UIManager starts.
            // This prevents a race condition where TurnController sets the state
            // *before* UIManager has subscribed to the event.
            if (AppGame.Instance != null && AppGame.Instance.StateMachine != null)
            {
                HandleStateChanged(AppGame.Instance.StateMachine.CurrentState);
            }
            else
            {
                Debug.LogError("UIManager: Could not sync to initial state. AppGame or StateMachine is null.");
            }
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
            
            GameEvents.OnSpellingQuizStarted -= HandleSpellingQuizStarted;
            GameEvents.OnPlayerInvested -= HandlePlayerInvested;
            GameEvents.OnQuizResult -= HandleQuizResult;
        }
        
        private void HandleStateChanged(GameState newState)
        {
            RollButton.interactable = (newState == GameState.PlayerTurn);
            
            if (newState != GameState.AwaitingPlayerMove)
                MoveNotificationPanel.SetActive(false);
            
            if (newState != GameState.AwaitingSpelling)
                SpellingQuizPanel.SetActive(false);
                
            // **FIXED:** Check for the correct state
            if (newState != GameState.ResolvingSpace)
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
            int index = playerID - 1;
            if (index >= 0 && index < MoneyTexts.Count)
            {
                MoneyTexts[index].text = $"P{playerID}: ${newBalance}";
            }
        }
        
        private void HandleSpellingQuizStarted(SpellingQuizPayload payload)
        {
            SpellingQuizPanel.SetActive(true);
            SpellingQuestionText.text = payload.Question.QuestionText;
            
            int currentPlayer = payload.PlayerID;
            int otherPlayer = (currentPlayer == 1) ? 2 : 1;
            int chances = AppGame.Instance.Investments.GetChancesLeft(otherPlayer);
            
            InvestButtonText.text = $"P{otherPlayer}: Invest? ({chances} left)";
            InvestButton.interactable = (chances > 0);
            
            CheckAnswerButton.gameObject.SetActive(true); 
        }

        private void HandlePlayerInvested(int playerID, string propertyName, int amount)
        {
            AddLog($"Player {playerID} invested ${amount} in {propertyName}!");
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
            if (LogText.text.Length > 1000)
            {
                LogText.text = LogText.text.Substring(0, 1000);
            }
        }
    }
}

