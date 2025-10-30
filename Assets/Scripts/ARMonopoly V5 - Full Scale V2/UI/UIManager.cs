using System.Collections.Generic;
using ARMonopoly_V5___Full_Scale_V2.Core;
using ARMonopoly_V5___Full_Scale_V2.Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ARMonopoly_V5___Full_Scale_V2.UI
{
    /// <summary>
    /// Replaces SimpleUI. Listens to all events and updates the UI.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [Header("HUD")]
        public TMP_Text TurnText;
        public TMP_Text DiceText;
        public Button RollButton;
        public TMP_Text NotificationText;
        public List<TMP_Text> MoneyTexts; // Assign in order of PlayerID (e.g., index 0 for P1, 1 for P2)

        [Header("Buy Panel")]
        public GameObject BuyPanel;
        public TMP_Text BuyLabel;
        public Button BuyButton;
        public Button PassButton;

        private BuyRequestPayload _pendingBuyRequest;

        void OnEnable()
        {
            GameEvents.OnGameStateChanged += HandleGameStateChanged;
            GameEvents.OnTurnStarted += HandleTurnStarted;
            GameEvents.OnDiceRolled += HandleDiceRolled;
            GameEvents.OnMoneyChanged += HandleMoneyChanged;
            GameEvents.OnBuyPrompt += HandleBuyPrompt;
            GameEvents.OnNotify += HandleNotify;

            BuyButton.onClick.AddListener(OnBuyClicked);
            PassButton.onClick.AddListener(OnPassClicked);
        }

        void OnDisable()
        {
            GameEvents.OnGameStateChanged -= HandleGameStateChanged;
            GameEvents.OnTurnStarted -= HandleTurnStarted;
            GameEvents.OnDiceRolled -= HandleDiceRolled;
            GameEvents.OnMoneyChanged -= HandleMoneyChanged;
            GameEvents.OnBuyPrompt -= HandleBuyPrompt;
            GameEvents.OnNotify -= HandleNotify;
            
            BuyButton.onClick.RemoveListener(OnBuyClicked);
            PassButton.onClick.RemoveListener(OnPassClicked);
        }

        void Start()
        {
            BuyPanel.SetActive(false);
            DiceText.text = "";
            HandleNotify("Game Started. Waiting for Player 1 to roll.");
        }

        void HandleGameStateChanged(GameState from, GameState to)
        {
            RollButton.interactable = (to == GameState.WaitingForRoll);
        }

        void HandleTurnStarted(int playerID)
        {
            TurnText.text = $"Player {playerID}'s Turn";
            DiceText.text = "Roll the dice!";
            HandleNotify($"Player {playerID}, \nit's your turn to roll.");
        }

        void HandleDiceRolled(int playerID, int d1, int d2)
        {
            DiceText.text = $"You rolled: {d1} + {d2}";
        }

        void HandleMoneyChanged(int playerID, int newBalance)
        {
            // Assumes PlayerID 1 is at index 0, PlayerID 2 at index 1...
            int index = playerID - 1;
            if (index >= 0 && index < MoneyTexts.Count)
            {
                MoneyTexts[index].text = $"P{playerID}: ${newBalance}";
            }
        }

        void HandleNotify(string message)
        {
            NotificationText.text = message;
        }

        void HandleBuyPrompt(BuyPromptPayload payload)
        {
            _pendingBuyRequest = new BuyRequestPayload 
            {
                PlayerID = payload.PlayerID, 
                PropertyID = payload.PropertyID
            };
            BuyLabel.text = $"Player {payload.PlayerID}, \nbuy {payload.DisplayName} for ${payload.Price}?";
            BuyPanel.SetActive(true);
        }

        void OnBuyClicked()
        {
            GameEvents.RaiseBuyRequested(_pendingBuyRequest);
            BuyPanel.SetActive(false);
        }

        void OnPassClicked()
        {
            // When the player passes, the RuleEngine needs to advance the turn.
            // We find the RuleEngine and tell it to advance.
            FindObjectOfType<RuleEngine>()?.AdvanceTurn();
            BuyPanel.SetActive(false);
        }
    }
}
