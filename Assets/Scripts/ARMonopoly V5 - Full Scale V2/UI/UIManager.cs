// In folder: ARMonopoly V5 - Full Scale V2/UI/
using System.Collections.Generic;
using ARMonopoly_V5___Full_Scale_V2.Core;
using ARMonopoly_V5___Full_Scale_V2.Gameplay;
using ARMonopoly_V5___Full_Scale_V2.Spelling;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ARMonopoly_V5___Full_Scale_V2.UI
{
    public class UIManager : MonoBehaviour
    {
        // --- Static Instance and Log method are REMOVED ---
        
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
        public Button SpellingSubmitButton;
        public Button ScanAnswerButton;
        public Button[] InvestmentButtons; 
        public ARCrosswordScanner crosswordScanner;

        [Header("Log")]
        public TextMeshProUGUI LogText;
        
        [Header("Fading Log")]
        public GameObject fadingLogPrefab; // The prefab we will create in Step 3
        public Transform fadingLogContainer; // The parent to hold the fading logs

        private string _pendingBuyPropertyID;

        private void Awake()
        {
            // --- Singleton logic is REMOVED ---
            
            if(BuyButton_Confirm) BuyButton_Confirm.onClick.AddListener(OnBuyConfirm);
            if(BuyButton_Pass) BuyButton_Pass.onClick.AddListener(OnBuyPass);
            if(SpellingSubmitButton) SpellingSubmitButton.onClick.AddListener(OnSpellingSubmit);
            if(ScanAnswerButton) ScanAnswerButton.onClick.AddListener(OnScanAnswer);

            BuyPanel.SetActive(false);
            // MoveNotificationPanel.SetActive(false);
            SpellingPanel.SetActive(false);
        }

        private void OnEnable()
        {
            // --- ADDED: Subscription to the new log event ---
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

        private void OnDisable()
        {
            // --- ADDED: Unsubscription from the new log event ---
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

        private void HandleStateChanged(GameState newState)
        {
            RollButton.interactable = (newState == GameState.PlayerTurn);
            // MoveNotificationPanel.SetActive(newState == GameState.AwaitingPlayerMove);
            BuyPanel.SetActive(newState == GameState.ResolvingSpace || newState == GameState.ResolvingSpelling);
            SpellingPanel.SetActive(newState == GameState.AwaitingSpellingAnswer);
            
            // We can still have the UI log its own events if we want
            // GameEvents.RaiseLogMessage($"Game state changed to: {newState}");
        }
        
        private void HandleSpellingQuestion(SpellingQuestionPayload payload)
        {
            SpellingQuestionText.text = payload.Question.QuestionText;
            SpellingAnswerInput.text = "";
            ScanAnswerButton.gameObject.SetActive(true);
            SpellingSubmitButton.gameObject.SetActive(true); 
            SpellingAnswerInput.gameObject.SetActive(true); 

            TurnController tc = FindObjectOfType<TurnController>();
            for(int i = 0; i < InvestmentButtons.Length; i++)
            {
                int playerID = i + 1;
                if (playerID != tc.CurrentPlayerID)
                {
                    InvestmentButtons[i].gameObject.SetActive(true);
                    InvestmentButtons[i].onClick.RemoveAllListeners();
                    int capturedInvestorID = playerID;
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
        
        // --- These event handlers are now just for UI updates ---
        private void OnPlayerPassedGo(int pid) { } // Log is handled by RuleEngine
        private void HandleTurnStarted(int playerID) 
        {
            MoveNotificationText.text = $"Player {playerID}, please roll the dice!";
            TurnText.text = $"Player {playerID}'s Turn";
            DiceRollText.text = "Roll the dice!";
        }
        private void HandleDiceRolled(int playerID, int totalRoll) 
        {
            DiceRollText.text = $"Player {playerID} rolled a {totalRoll}!";
        }
        private void HandleMoveRequired(MovePayload payload) 
        {
            MoveNotificationText.text = $"Player {payload.PlayerID}, please move to:\n{payload.DestinationName}";
        }
        private void HandlePropertyBought(PropertyPayload payload) { } // Log is handled by RuleEngine
        private void HandleRentPaid(RentPayload payload) { } // Log is handled by Bank
        
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
            GameEvents.RaiseLogMessage("Player passed on purchase.");
        }

        // --- This is now a private method, just for this class ---
        private void AddLog(string message)
        {
            // --- 1. Your existing logic to update the persistent log ---
            Debug.Log(message);
            if (LogText != null)
            {
                LogText.text = message + "\n" + LogText.text;
                if (LogText.text.Length > 1000)
                {
                    LogText.text = LogText.text.Substring(0, 1000);
                }
            }

            // --- 2. NEW logic to spawn the fading message ---
            if (fadingLogPrefab != null && fadingLogContainer != null)
            {
                GameObject logGO = Instantiate(fadingLogPrefab, fadingLogContainer);
                logGO.GetComponent<FadingLogMessage>().Initialize(message);
            }
            // --- END NEW ---
        }
    }
}