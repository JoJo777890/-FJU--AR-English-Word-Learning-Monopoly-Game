using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ARMonopoly_Medium_Scale
{
    public class SimpleUI : MonoBehaviour
    {
        [Header("HUD")]
        public TMP_Text turnText;
        public TMP_Text diceText;
        public TMP_Text[] moneyTexts; // index by playerId
        public TMP_Text distancesText;
        public TMP_Text logText;

        [Header("Buy")]
        public GameObject buyPanel;
        public TMP_Text buyLabel;
        public Button buyBtn;

        [Header("Cards")]
        public GameObject cardPanel;
        public TMP_Text cardTitle;
        public TMP_Text cardBody;
        public Button cardOkBtn;

        int pendingPlayerId = -1; string pendingPropId = null;

        void Awake() { HideBuy(); HideCard(); }

        void OnEnable()
        {
            GameEvents.TurnStarted      += OnTurn;
            GameEvents.DiceRolled       += OnDice;
            GameEvents.MoneyChanged     += OnMoney;
            GameEvents.BuyPrompt        += OnBuyPrompt;
            GameEvents.PropertyBought   += e => Log($"P{e.playerId} bought {e.propertyName} (${e.price}).");
            GameEvents.RentPaid         += e => Log($"P{e.payerId} paid ${e.amount} to P{e.ownerId} ({e.propertyName}).");
            GameEvents.DistancesUpdated += s => { if (distancesText) distancesText.text = s; };
            GameEvents.CardDrawn        += OnCardDrawn;
        }
        void OnDisable()
        {
            GameEvents.TurnStarted      -= OnTurn;
            GameEvents.DiceRolled       -= OnDice;
            GameEvents.MoneyChanged     -= OnMoney;
            GameEvents.BuyPrompt        -= OnBuyPrompt;
            GameEvents.DistancesUpdated -= (s)=>{};
            GameEvents.CardDrawn        -= OnCardDrawn;
        }

        void OnTurn(int pid)
        {
            if (turnText)
            {
                turnText.text = $"Turn: P{pid}";
            }
        }

        void OnDice(int pid, int d1, int d2)
        {
            if (diceText)
            {
                diceText.text = $"Dice: {d1}+{d2}";
            }
        }

        void OnMoney(MoneyChanged e)
        {
            if (moneyTexts != null && e.playerId >= 0 && e.playerId < moneyTexts.Length && moneyTexts[e.playerId])
            {
                moneyTexts[e.playerId].text = $"P{e.playerId}: ${e.money}";
            }
        }

        void OnBuyPrompt(BuyPrompt e)
        {
            Debug.Log($"[UI] Showing BuyPanel for P{e.playerId} {e.propertyName} ${e.price}"); // --Debug
            
            pendingPlayerId = e.playerId; pendingPropId = e.propertyId;
            if (buyPanel)
            {
                buyPanel.SetActive(true);
            }

            if (buyLabel)
            {
                buyLabel.text = $"P{e.playerId}: Buy {e.propertyName} for ${e.price}?";
            }
            buyBtn.onClick.RemoveAllListeners();
            buyBtn.onClick.AddListener(() => {
                GameEvents.RaiseBuyRequested(new BuyRequest
                {
                    playerId=pendingPlayerId, 
                    propertyId=pendingPropId
                });
                HideBuy();
            });
        }

        void OnCardDrawn(CardDrawn e)
        {
            if (cardPanel) 
                cardPanel.SetActive(true);
            
            if (cardTitle) 
                cardTitle.text = $"{e.deck}: {e.title}";
            
            if (cardBody)  
                cardBody.text  = e.body;
            
            if (cardOkBtn)
            {
                cardOkBtn.onClick.RemoveAllListeners();
                cardOkBtn.onClick.AddListener(HideCard);
            }
        }

        void HideBuy()
        {
            if (buyPanel) 
                buyPanel.SetActive(false);
        }

        void HideCard()
        {
            if (cardPanel) 
                cardPanel.SetActive(false);
        }

        void Log(string s)
        {
            if (logText) 
                logText.text = s + "\n" + logText.text;
        }
    }
}
