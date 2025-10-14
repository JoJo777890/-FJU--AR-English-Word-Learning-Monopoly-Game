// Assets/Scripts/Simple/SimpleUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ARMonopoly.Simple
{
    public class SimpleUI : MonoBehaviour
    {
        [Header("Refs")]
        public TMP_Text distancesText;
        public TMP_Text logText;

        [Header("Buy Panel")]
        public GameObject buyPanel;
        public TMP_Text buyLabel;
        public Button buyButton;

        [Header("Money (optional)")]
        public TMP_Text[] moneyTexts; // index by playerId

        // state for buy action
        private int pendingPlayerId = -1;
        private string pendingPropId = null;

        private void OnEnable()
        {
            GameEvents.DistancesUpdated += SetDistances;
            GameEvents.BuyPrompt        += OnBuyPrompt;
            GameEvents.PropertyBought   += OnPropertyBought;
            GameEvents.RentPaid         += OnRentPaid;
            GameEvents.MoneyChanged     += OnMoneyChanged;
        }
        private void OnDisable()
        {
            GameEvents.DistancesUpdated -= SetDistances;
            GameEvents.BuyPrompt        -= OnBuyPrompt;
            GameEvents.PropertyBought   -= OnPropertyBought;
            GameEvents.RentPaid         -= OnRentPaid;
            GameEvents.MoneyChanged     -= OnMoneyChanged;
        }

        private void Awake() => HideBuy();

        private void SetDistances(string s) 
        {
            if (distancesText) 
                distancesText.text = s;
        }

        private void OnBuyPrompt(BuyPrompt e)
        {
            pendingPlayerId = e.playerId;
            pendingPropId   = e.propertyId;

            if (buyPanel) 
                buyPanel.SetActive(true);
            if (buyLabel) 
                buyLabel.text = $"P{e.playerId}: Buy {e.propertyName} for ${e.price}?";

            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(() =>
            {
                GameEvents.RaiseBuyRequested(new BuyRequest
                {
                    playerId = pendingPlayerId, 
                    propertyId = pendingPropId
                });
                HideBuy();
            });
        }

        private void OnPropertyBought(PropertyBought e)
        {
            AppendLog($"P{e.playerId} bought {e.propertyName} for ${e.price}.");
        }

        private void OnRentPaid(RentPaid e)
        {
            AppendLog($"P{e.payerId} paid ${e.amount} rent to P{e.ownerId} for {e.propertyName}.");
        }

        private void OnMoneyChanged(MoneyChanged e)
        {
            if (moneyTexts != null && e.playerId >= 0 && e.playerId < moneyTexts.Length && moneyTexts[e.playerId] != null)
                moneyTexts[e.playerId].text = $"P{e.playerId}: ${e.money}";
        }

        private void HideBuy() 
        {
            if (buyPanel) 
                buyPanel.SetActive(false);
        }
        private void AppendLog(string s)
        {
            if (!logText) return;
            logText.text = s + "\n" + logText.text;
        }
    }
}
