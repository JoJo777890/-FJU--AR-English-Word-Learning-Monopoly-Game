// In folder: ARMonopoly V5 - Full Scale V2/UI/
using System.Collections.Generic;
using ARMonopoly_V5___Full_Scale_V2.Core;
using ARMonopoly_V5___Full_Scale_V2.Gameplay;
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
        public GameObject SpellingPanel; // New

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
        
        [Header("Spelling Panel")] // New
        public TextMeshProUGUI SpellingQuestionText;
        public TMP_InputField SpellingAnswerInput;
        public Button SpellingSubmitButton;
        public Button[] InvestmentButtons; // Assign buttons for players who can invest
        

        [Header("Log")]
        public TextMeshProUGUI LogText;

        private string _pendingBuyPropertyID;

        private void Awake()
        {
            if(BuyButton_Confirm) BuyButton_Confirm.onClick.AddListener(OnBuyConfirm);
            if(BuyButton_Pass) BuyButton_Pass.onClick.AddListener(OnBuyPass);
            if(SpellingSubmitButton) SpellingSubmitButton.onClick.AddListener(OnSpellingSubmit);

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

            // Configure investment buttons
            // This is a simple example for a 2-player game. You can expand this logic.
            TurnController tc = FindObjectOfType<TurnController>();
            for(int i = 0; i < InvestmentButtons.Length; i++)
            {
                int playerID = i + 1; // Assuming player IDs are 1-based
                if (playerID != tc.CurrentPlayerID)
                {
                    InvestmentButtons[i].gameObject.SetActive(true);
                    InvestmentButtons[i].onClick.RemoveAllListeners();
                    InvestmentButtons[i].onClick.AddListener(() => {
                        GameEvents.RaisePlayerInvest(new InvestmentPayload { InvestorID = playerID, TargetPlayerID = tc.CurrentPlayerID });
                        InvestmentButtons[playerID - 1].gameObject.SetActive(false); // Disable after investing
                    });
                }
                else
                {
                    InvestmentButtons[i].gameObject.SetActive(false);
                }
            }
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