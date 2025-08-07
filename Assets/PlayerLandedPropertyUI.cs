using TMPro;
using UnityEngine;

public class PlayerLandedPropertyUI : MonoBehaviour
{
    public static PlayerLandedPropertyUI Instance { get; private set; }

    public TextMeshProUGUI infoText; // Assign this in the Inspector

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Prevent duplicates
        }
        else
        {
            Instance = this;
        }
    }

    public void UpdateUI (string text)
    {
        infoText.text = "[Landed At]\n";
        infoText.text += text;
    }
}