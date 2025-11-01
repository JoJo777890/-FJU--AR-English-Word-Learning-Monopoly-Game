using System.Collections.Generic;
using ARMonopoly_V5___Full_Scale_V2.Core;
using ARMonopoly_V5___Full_Scale_V2.Economy;
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

        [Header("Log")]
        public TextMeshProUGUI LogText;

        private string _pendingBuyPropertyID;
        private bool _isBuyPromptActive = false;

        // Awake is called once when the script instance is being loaded.
        // Best for getting references and wiring its own components.
        private void Awake()
        {
            // Button listeners only need to be set up once.
            if(BuyButton_Confirm)
                BuyButton_Confirm.onClick.AddListener(OnBuyConfirm);
            if(BuyButton_Pass)
                BuyButton_Pass.onClick.AddListener(OnBuyPass);

            // Initial UI state
            if(BuyPanel) BuyPanel.SetActive(false);
            if(MoveNotificationPanel) MoveNotificationPanel.SetActive(false);
            if(DiceRollText) DiceRollText.text = "";
        }

        // OnEnable is called when the object becomes enabled and active.
        // Best for subscribing to external events.
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
        }

        // OnDisable is called when the object becomes disabled.
        // Best for unsubscribing from external events.
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
        }

        // // Start is called before the first frame update.
        // // Good for initialization that depends on other objects.
        // private void Start()
        // {
        //     // Initialize Money display after AppGame/Bank are ready.
        //     InitializeMoneyDisplays();
        // }
        //
        // private void InitializeMoneyDisplays()
        // {
        //     if (AppGame.Instance == null || AppGame.Instance.Bank == null)
        //     {
        //         Debug.LogWarning("UIManager: AppGame or Bank not ready for money initialization!");
        //         return;
        //     }
        //
        //     foreach (var playerTag in FindObjectsOfType<Scene.PlayerTag>())
        //     {
        //         Wallet wallet = AppGame.Instance.Bank.GetWallet(playerTag.PlayerID);
        //         if (wallet != null)
        //             HandleMoneyChanged(playerTag.PlayerID, wallet.Money);
        //         else
        //             Debug.LogWarning($"UIManager: Could not find wallet for Player {playerTag.PlayerID} during initialization.");
        //     }
        // }

        private void OnPlayerPassedGo(int pid)
        {
            if (AppGame.Instance.Config != null)
            {
                AddLog($"Player {pid} passed GO! Received ${AppGame.Instance.Config.PassGoMoney}");
            }
        }

        private void HandleStateChanged(GameState newState)
        {
            // Control UI visibility based on state
            if(RollButton)
                RollButton.interactable = (newState == GameState.PlayerTurn);
            
            if(MoveNotificationPanel)
                MoveNotificationPanel.SetActive(newState == GameState.AwaitingPlayerMove);

            // Only show buy panel if we are in the resolving state AND a buy prompt is active
            if(BuyPanel)
                BuyPanel.SetActive(newState == GameState.ResolvingSpace && _isBuyPromptActive);
        }

        private void HandleTurnStarted(int playerID)
        {
            if(TurnText)
                TurnText.text = $"Player {playerID}'s Turn";
            if(DiceRollText)
                DiceRollText.text = "Roll the dice!";
            AddLog($"Player {playerID}'s turn has started.");
        }

        private void HandleDiceRolled(int playerID, int totalRoll)
        {
            if(DiceRollText)
                DiceRollText.text = $"Player {playerID} rolled a {totalRoll}!";
        }

        private void HandleMoveRequired(MovePayload payload)
        {
            if(MoveNotificationText)
                MoveNotificationText.text = $"Player {payload.PlayerID}, please move your token to:\n{payload.DestinationName}";
            AddLog($"Waiting for Player {payload.PlayerID} to move to {payload.DestinationName}.");
        }

        private void HandleMoneyChanged(int playerID, int newBalance)
        {
            // Assumes Player 1 is at index 0, Player 2 at index 1, etc.
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
            _isBuyPromptActive = true;
            if(BuyPromptText)
                BuyPromptText.text = $"Player {payload.PlayerID}, buy {payload.PropertyName} for ${payload.Price}?"; // <-- FIXED: Was payload.PricePrice

            // The HandleStateChanged method will now show the panel
            // because the state is ResolvingSpace and _isBuyPromptActive is true.
            HandleStateChanged(AppGame.Instance.StateMachine.CurrentState);
        }

        private void OnBuyConfirm()
        {
            if (AppGame.Instance.StateMachine.CurrentState != GameState.ResolvingSpace) return;

            _isBuyPromptActive = false;
            if(BuyPanel) BuyPanel.SetActive(false);
            GameEvents.RaiseBuyRequest(_pendingBuyPropertyID);
            AddLog($"Player purchasing {_pendingBuyPropertyID}.");
        }

        private void OnBuyPass()
        {
            if (AppGame.Instance.StateMachine.CurrentState != GameState.ResolvingSpace) return;

            _isBuyPromptActive = false;
            if(BuyPanel) BuyPanel.SetActive(false);

            // We need to tell the TurnController to end the turn
            TurnController tc = FindObjectOfType<TurnController>();
            if (tc != null)
                tc.EndTurn();
            else
                Debug.LogError("UIManager: Could not find TurnController to end turn on 'Pass'.");


            AddLog("Player passed on purchase.");
        }

        private void HandlePropertyBought(PropertyPayload payload)
        {
            AddLog($"Player {payload.PlayerID} bought {payload.PropertyName}!");
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
                if (LogText.text.Length > 1000) // Prune log
                {
                    LogText.text = LogText.text.Substring(0, 1000);
                }
            }
        }
    }
}

