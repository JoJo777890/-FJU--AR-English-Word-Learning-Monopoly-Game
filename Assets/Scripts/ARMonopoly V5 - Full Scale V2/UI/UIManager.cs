using System.Collections.Generic;
using ARMonopoly_V5___Full_Scale_V2.Core;
using ARMonopoly_V5___Full_Scale_V2.Gameplay;
using ARMonopoly_V5___Full_Scale_V2.Spelling;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ARMonopoly_V5___Full_Scale_V2.UI
{
    /// <summary>
    /// Main UI controller. Manages all UI panels, buttons, and text fields.
    /// Subscribes to GameEvents to update the UI based on game state.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [Header("State Panels")]
        [Tooltip("Panel for prompting property purchase.")]
        public GameObject BuyPanel;
        [Tooltip("Panel that instructs the player to move their token.")]
        public GameObject MoveNotificationPanel;
        [Tooltip("Panel for the spelling challenge and investment.")]
        public GameObject SpellingPanel; 

        [Header("HUD")]
        public Button RollButton;
        public TextMeshProUGUI TurnText;
        public TextMeshProUGUI DiceRollText;
        [Tooltip("List of text elements for player money, index 0 = Player 1, index 1 = Player 2, etc.")]
        public List<TextMeshProUGUI> MoneyTexts;

        [Header("Buy Panel")]
        public TextMeshProUGUI BuyPromptText;
        public Button BuyButton_Confirm;
        public Button BuyButton_Pass;

        [Header("Move Panel")]
        public TextMeshProUGUI MoveNotificationText;
        
        [Header("Spelling Panel")] 
        public TextMeshProUGUI SpellingQuestionText;
        public TMP_InputField SpellingAnswerInput;
        public Button SpellingSubmitButton;
        public Button ScanAnswerButton;
        [Tooltip("Buttons for other players to invest. Index 0 = P1, 1 = P2, etc.")]
        public Button[] InvestmentButtons; 
        [Tooltip("Reference to the ARCrosswordScanner in the scene.")]
        public ARCrosswordScanner crosswordScanner;

        [Header("Log")]
        [Tooltip("The persistent, scrolling log text field.")]
        public TextMeshProUGUI LogText;
        
        [Header("Fading Log")]
        [Tooltip("The prefab for the fading pop-up log message.")]
        public GameObject fadingLogPrefab;
        [Tooltip("The UI container to instantiate fading logs into.")]
        public Transform fadingLogContainer;

        private string _pendingBuyPropertyID;

        /// <summary>
        /// Caches references and hooks up permanent button listeners.
        /// </summary>
        private void Awake()
        {
            if(BuyButton_Confirm) BuyButton_Confirm.onClick.AddListener(OnBuyConfirm);
            if(BuyButton_Pass) BuyButton_Pass.onClick.AddListener(OnBuyPass);
            if(SpellingSubmitButton) SpellingSubmitButton.onClick.AddListener(OnSpellingSubmit);
            if(ScanAnswerButton) ScanAnswerButton.onClick.AddListener(OnScanAnswer);

            // Set initial UI state
            BuyPanel.SetActive(false);
            MoveNotificationPanel.SetActive(true); // Keep this on by default
            SpellingPanel.SetActive(false);
        }

        /// <summary>
        /// Subscribes to all game events.
        /// </summary>
        private void OnEnable()
        {
            GameEvents.OnLogMessage += AddLog;
            GameEvents.OnStateChanged += HandleStateChanged;
            GameEvents.OnTurnStarted += HandleTurnStarted;
            GameEvents.OnDiceRolled += HandleDiceRolled;
            GameEvents.OnMoneyChanged += HandleMoneyChanged;
            GameEvents.OnBuyPrompt += HandleBuyPrompt;
            GameEvents.OnPropertyBought += HandlePropertyBought;
            GameEvents.OnRentPaid += HandleRentPaid;
            GameEvents.OnMoveRequired += HandleMoveRequired;
            GameEvents.OnPlayerPassedGo += OnPlayerPassedGo;
            GameEvents.OnSpellingQuestion += HandleSpellingQuestion;
        }

        /// <summary>
        /// Unsubscribes from all game events.
        /// </summary>
        private void OnDisable()
        {
            GameEvents.OnLogMessage -= AddLog;
            GameEvents.OnStateChanged -= HandleStateChanged;
            GameEvents.OnTurnStarted -= HandleTurnStarted;
            GameEvents.OnDiceRolled -= HandleDiceRolled;
            GameEvents.OnMoneyChanged -= HandleMoneyChanged;
            GameEvents.OnBuyPrompt -= HandleBuyPrompt;
            GameEvents.OnPropertyBought -= HandlePropertyBought;
            GameEvents.OnRentPaid -= HandleRentPaid;
            GameEvents.OnMoveRequired -= HandleMoveRequired;
            GameEvents.OnPlayerPassedGo -= OnPlayerPassedGo;
            GameEvents.OnSpellingQuestion -= HandleSpellingQuestion;
        }

        #region Event Handlers

        /// <summary>
        /// Updates UI visibility based on the new game state.
        /// </summary>
        private void HandleStateChanged(GameState newState)
        {
            RollButton.interactable = (newState == GameState.PlayerTurn);
            // MoveNotificationPanel.SetActive(newState == GameState.AwaitingPlayerMove);
            BuyPanel.SetActive(newState == GameState.ResolvingSpace || newState == GameState.ResolvingSpelling);
            SpellingPanel.SetActive(newState == GameState.AwaitingSpellingAnswer);
        }
        
        /// <summary>
        /// Configures and displays the spelling panel.
        /// </summary>
        private void HandleSpellingQuestion(SpellingQuestionPayload payload)
        {
            SpellingQuestionText.text = payload.Question.QuestionText;
            SpellingAnswerInput.text = "";

            // Show both buttons, allowing multiple scans before submission.
            ScanAnswerButton.gameObject.SetActive(true);
            SpellingSubmitButton.gameObject.SetActive(true); 
            SpellingAnswerInput.gameObject.SetActive(true); 

            // Configure investment buttons
            TurnController tc = FindObjectOfType<TurnController>();
            for(int i = 0; i < InvestmentButtons.Length; i++)
            {
                int playerID = i + 1; // Assumes player IDs are 1-based
                if (playerID != tc.CurrentPlayerID)
                {
                    InvestmentButtons[i].gameObject.SetActive(true);
                    InvestmentButtons[i].onClick.RemoveAllListeners();
                    int capturedInvestorID = playerID; // Capture variable for closure
                    InvestmentButtons[i].onClick.AddListener(() => {
                        GameEvents.RaisePlayerInvest(new InvestmentPayload { InvestorID = capturedInvestorID, TargetPlayerID = tc.CurrentPlayerID });
                        InvestmentButtons[capturedInvestorID - 1].gameObject.SetActive(false); // Disable after investing
                    });
                }
                else
                {
                    InvestmentButtons[i].gameObject.SetActive(false); // Hide button for the current player
                }
            }
        }
        
        /// <summary>
        /// Updates the HUD text when a new turn starts.
        /// </summary>
        private void HandleTurnStarted(int playerID) 
        {
            MoveNotificationText.text = $"Player {playerID}, please roll the dice!";
            TurnText.text = $"Player {playerID}'s Turn";
            DiceRollText.text = "Roll the dice!";
        }
        
        /// <summary>
        /// Updates the HUD text with the dice roll result.
        /// </summary>
        private void HandleDiceRolled(int playerID, int totalRoll) 
        {
            DiceRollText.text = $"Player {playerID} rolled a {totalRoll}!";
        }
        
        /// <summary>
        /// Updates the MoveNotificationPanel with the destination.
        /// </summary>
        private void HandleMoveRequired(MovePayload payload) 
        {
            MoveNotificationText.text = $"Player {payload.PlayerID}, please move to:\n{payload.DestinationName}";
        }
        
        /// <summary>
        /// Updates the money display for a specific player.
        /// </summary>
        private void HandleMoneyChanged(int playerID, int newBalance)
        {
            int moneyIndex = playerID - 1; // Assumes Player 1 is at index 0
            if (moneyIndex >= 0 && moneyIndex < MoneyTexts.Count)
            {
                if (MoneyTexts[moneyIndex] != null)
                    MoneyTexts[moneyIndex].text = $"P{playerID}: ${newBalance}";
            }
        }

        /// <summary>
        /// Configures and displays the BuyPanel.
        /// </summary>
        private void HandleBuyPrompt(BuyPayload payload)
        {
            _pendingBuyPropertyID = payload.PropertyID;
            BuyPromptText.text = $"Player {payload.PlayerID}, buy {payload.PropertyName} for ${payload.Price}?";
        }

        // These handlers are empty because the log message is raised by the source (RuleEngine, Bank)
        private void OnPlayerPassedGo(int pid) { } 
        private void HandlePropertyBought(PropertyPayload payload) { } 
        private void HandleRentPaid(RentPayload payload) { } 

        #endregion

        #region UI Callbacks

        /// <summary>
        /// Called by the "Scan Answer" button.
        /// </summary>
        private void OnScanAnswer()
        {
            if (crosswordScanner == null)
            {
                GameEvents.RaiseLogMessage("Crossword Scanner not assigned to UIManager!");
                return;
            }
            string scannedWord = crosswordScanner.GetFirstHorizontalWord();
            SpellingAnswerInput.text = scannedWord;
            GameEvents.RaiseLogMessage($"Scanned word: {scannedWord}");
        }
        
        /// <summary>
        /// Called by the "Submit" button on the SpellingPanel.
        /// </summary>
        private void OnSpellingSubmit()
        {
            TurnController tc = FindObjectOfType<TurnController>();
            GameEvents.RaiseSpellingAnswer(new SpellingAnswerPayload
            {
                PlayerID = tc.CurrentPlayerID,
                Answer = SpellingAnswerInput.text
            });
            SpellingPanel.SetActive(false);
            GameEvents.RaiseLogMessage("Spelling answer submitted.");
        }

        /// <summary>
        /// Called by the "Confirm" button on the BuyPanel.
        /// </summary>
        private void OnBuyConfirm()
        {
            GameEvents.RaiseBuyRequest(_pendingBuyPropertyID);
            BuyPanel.SetActive(false);
        }

        /// <summary>
        /// Called by the "Pass" button on the BuyPanel.
        /// </summary>
        private void OnBuyPass()
        {
            FindObjectOfType<TurnController>().EndTurn();
            BuyPanel.SetActive(false);
            GameEvents.RaiseLogMessage("Player passed on purchase.");
        }

        #endregion

        /// <summary>
        /// Handles the OnLogMessage event, updating both the persistent log
        /// and spawning a fading log message.
        /// </summary>
        private void AddLog(string message)
        {
            // 1. Update the persistent log
            Debug.Log(message);
            if (LogText != null)
            {
                LogText.text = message + "\n" + LogText.text;
                // Prune the log to prevent it from getting too long
                if (LogText.text.Length > 1000)
                {
                    LogText.text = LogText.text.Substring(0, 1000);
                }
            }

            // 2. Spawn the fading message
            if (fadingLogPrefab != null && fadingLogContainer != null)
            {
                GameObject logGO = Instantiate(fadingLogPrefab, fadingLogContainer);
                logGO.GetComponent<FadingLogMessage>().Initialize(message);
            }
        }
    }
}