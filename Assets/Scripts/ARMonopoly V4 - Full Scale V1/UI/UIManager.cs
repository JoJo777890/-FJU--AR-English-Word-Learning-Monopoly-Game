using System;
using ARMonopoly_V4___Full_Scale_V1.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ARMonopoly_V4___Full_Scale_V1.UI
{
    public class UIManager : MonoBehaviour
    {
        [Header("Start Game")]
        public GameObject startGameUI;
        public Button startBtn;

        private void OnEnable()
        {
            GameEvents.GameStart += OnStartMenu;
        }
        private void OnDisable()
        {
            GameEvents.GameStart -= OnStartMenu;
        }

        private void OnStartMenu()
        {
            startBtn.onClick.RemoveAllListeners();
            startBtn.onClick.AddListener(() =>
            {
                HideStartMenu();
            });
        }

        private void HideStartMenu()
        {
            startGameUI.SetActive(false);
        }
    }
}
