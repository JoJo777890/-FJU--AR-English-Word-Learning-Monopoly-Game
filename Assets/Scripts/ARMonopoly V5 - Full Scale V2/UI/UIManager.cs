// In folder: ARMonopoly V5 - Full Scale V2/UI/
using System.Collections.Generic;
using ARMonopoly_V5___Full_Scale_V2.Core;
using ARMonopoly_V5___Full_Scale_V2.Gameplay;
using ARMonopoly_V5___Full_Scale_V2.Spelling; // <-- Add this namespace
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ARMonopoly_V5___Full_Scale_V2.UI
{
    public class UIManager : MonoBehaviour
    {
        [Header("State Panels")]
        public GameObject BuyPanel;
        public GameObject MoveNotificationPanel;
        public GameObject SpellingPanel;

        [Header("HUD")]
        public Button RollButton;
        public TextMeshProUGUI TurnText;
        public TextMeshProUGUI DiceRollText;
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
        public Button SpellingSubmitButton; // This is the "Confirm" button
        public Button ScanAnswerButton;     // New button to trigger the scan
        public Button[] InvestmentButtons;
        public ARCrosswordScanner crosswordScanner; // New: Reference to the scanner

        [Header("Log")]
        public TextMeshProUGUI LogText;

        private string _pendingBuyPropertyID;

        private void Awake()
        {
            if(BuyButton_Confirm) BuyButton_Confirm.onClick.AddListener(OnBuyConfirm);
            if(BuyButton_Pass) BuyButton_Pass.onClick.AddListener(OnBuyPass);
            if(SpellingSubmitButton) SpellingSubmitButton.onClick.AddListener(OnSpellingSubmit);
            if(ScanAnswerButton) ScanAnswerButton.onClick.AddListener(OnScanAnswer); // New

            BuyPanel.SetActive(false);
            MoveNotificationPanel.SetActive(false);
            SpellingPanel.SetActive(false);
        }

        private void OnEnable()
        {
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

        private void OnDisable()
        {
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

        private void HandleStateChanged(GameState newState)
        {
            RollButton.interactable = (newState == GameState.PlayerTurn);
            MoveNotificationPanel.SetActive(newState == GameState.AwaitingPlayerMove);
            BuyPanel.SetActive(newState == GameState.ResolvingSpace || newState == GameState.ResolvingSpelling);
            SpellingPanel.SetActive(newState == GameState.AwaitingSpellingAnswer);
        }
        
        private void HandleSpellingQuestion(SpellingQuestionPayload payload)
        {
            SpellingQuestionText.text = payload.Question.QuestionText;
            SpellingAnswerInput.text = "";

            // Show scan button, hide submit button
            ScanAnswerButton.gameObject.SetActive(true);
            SpellingSubmitButton.gameObject.SetActive(false);
            SpellingAnswerInput.gameObject.SetActive(true); // Keep this visible to show the result

            // Configure investment buttons
            TurnController tc = FindObjectOfType<TurnController>();
            for(int i = 0; i < InvestmentButtons.Length; i++)
            {
                int playerID = i + 1; // Assuming player IDs are 1-based
                if (playerID != tc.CurrentPlayerID)
                {
                    InvestmentButtons[i].gameObject.SetActive(true);
                    InvestmentButtons[i].onClick.RemoveAllListeners();
                    int capturedInvestorID = playerID; // Capture variable for listener
                    InvestmentButtons[i].onClick.AddListener(() => {
                        GameEvents.RaisePlayerInvest(new InvestmentPayload { InvestorID = capturedInvestorID, TargetPlayerID = tc.CurrentPlayerID });
                        InvestmentButtons[capturedInvestorID - 1].gameObject.SetActive(false);
                    });
                }
                else
                {
                    InvestmentButtons[i].gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// Called when the "Scan Answer" button is clicked.
        /// </summary>
        private void OnScanAnswer()
        {
            if (crosswordScanner == null)
            {
                AddLog("Crossword Scanner not assigned to UIManager!");
                return;
            }

            // Run the scan and get the first word
            string scannedWord = crosswordScanner.GetFirstHorizontalWord();
            SpellingAnswerInput.text = scannedWord;
            AddLog($"Scanned word: {scannedWord}");

            // Hide scan button, show submit button
            ScanAnswerButton.gameObject.SetActive(false);
            SpellingSubmitButton.gameObject.SetActive(true);
        }

        /// <summary>
        /// Called when the "Submit" (confirm) button is clicked.
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
        }


        // ... (rest of the UIManager code is the same)

        private void OnPlayerPassedGo(int pid)
        {
            if (AppGame.Instance.Config != null)
            {
                AddLog($"Player {pid} passed GO! Received ${AppGame.Instance.Config.PassGoMoney}");
            }
        }

        private void HandleTurnStarted(int playerID)
        {
            TurnText.text = $"Player {playerID}'s Turn";
            DiceRollText.text = "Roll the dice!";
            AddLog($"Player {playerID}'s turn has started.");
        }

        private void HandleDiceRolled(int playerID, int totalRoll)
        {
            DiceRollText.text = $"Player {playerID} rolled a {totalRoll}!";
        }

        private void HandleMoveRequired(MovePayload payload)
        {
            MoveNotificationText.text = $"Player {payload.PlayerID}, please move to:\n{payload.DestinationName}";
            AddLog($"Waiting for Player {payload.PlayerID} to move to {payload.DestinationName}.");
        }

        private void HandleMoneyChanged(int playerID, int newBalance)
        {
            int moneyIndex = playerID - 1;
            if (moneyIndex >= 0 && moneyIndex < MoneyTexts.Count)
            {
                if (MoneyTexts[moneyIndex] != null)
                    MoneyTexts[moneyIndex].text = $"P{playerID}: ${newBalance}";
            }
        }

        private void HandleBuyPrompt(BuyPayload payload)
        {
            _pendingBuyPropertyID = payload.PropertyID;
            BuyPromptText.text = $"Player {payload.PlayerID}, buy {payload.PropertyName} for ${payload.Price}?";
        }

        private void OnBuyConfirm()
        {
            GameEvents.RaiseBuyRequest(_pendingBuyPropertyID);
            BuyPanel.SetActive(false);
        }

        private void OnBuyPass()
        {
            FindObjectOfType<TurnController>().EndTurn();
            BuyPanel.SetActive(false);
            AddLog("Player passed on purchase.");
        }

        private void HandlePropertyBought(PropertyPayload payload)
        {
            AddLog($"Player {payload.PlayerID} bought {payload.PropertyName} for ${payload.Price}!");
        }

        private void HandleRentPaid(RentPayload payload)
        {
            AddLog($"Player {payload.PayerID} paid ${payload.Amount} rent to Player {payload.OwnerID} for {payload.PropertyName}.");
        }

        private void AddLog(string message)
        {
            Debug.Log(message);
            if (LogText != null)
            {
                LogText.text = message + "\n" + LogText.text;
                if (LogText.text.Length > 1000)
                {
                    LogText.text = LogText.text.Substring(0, 1000);
                }
            }
        }
    }
}