// Assets/Scripts/Simple/SimpleUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ARMonopoly.Simple
{
    public class SimpleUI : MonoBehaviour
    {
        public static SimpleUI Instance;

        [Header("Refs")]
        public TMP_Text distancesText;
        public TMP_Text logText;
        public GameObject buyPanel;
        public TMP_Text buyLabel;
        public Button buyButton;

        [Header("Money (optional)")]
        public TMP_Text[] moneyTexts; // index by playerId

        private void Awake()
        {
            Instance = this;
            HideBuy();
        }

        public static void UpdateDistances(string s) => Instance?.SetDistances(s);
        public static void Log(string s) => Instance?.AppendLog(s);
        public static void ShowBuy(PropertyTag prop, PlayerTag player, System.Action onBuy)
        {
            if (Instance == null) return;
            Instance.buyPanel.SetActive(true);
            Instance.buyLabel.text = $"{player.playerName}: Buy {prop.displayName} for ${prop.price}?";
            Instance.buyButton.onClick.RemoveAllListeners();
            Instance.buyButton.onClick.AddListener(() =>
            {
                onBuy?.Invoke();
                Instance.HideBuy();
            });
        }
        public static void RefreshMoney(int playerId, int money)
        {
            if (Instance == null) return;
            if (playerId >= 0 && playerId < Instance.moneyTexts.Length && Instance.moneyTexts[playerId] != null)
                Instance.moneyTexts[playerId].text = $"P{playerId}: ${money}";
        }

        private void SetDistances(string s)
        {
            if (distancesText) distancesText.text = s;
        }

        private void AppendLog(string s)
        {
            if (!logText) return;
            logText.text = s + "\n" + logText.text;
        }

        private void HideBuy()
        {
            if (buyPanel) buyPanel.SetActive(false);
        }
    }
}