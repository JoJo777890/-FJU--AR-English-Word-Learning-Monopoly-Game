using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Vuforia;

public class PlayerTokenTrigger : MonoBehaviour
{
    public List<ObserverBehaviour> propertyTargets;
    public float triggerDistance = 0.3f;

    private string PlayerLandedPropertyUIText;

    private ObserverBehaviour playerObserver;

    void Start()
    {
        playerObserver = GetComponent<ObserverBehaviour>();
    }

    void Update()
    {
        PlayerLandedPropertyUIText = "";

        if (playerObserver == null || playerObserver.TargetStatus.Status < Status.TRACKED)
            return;

        int distanceNum = 0;

        foreach (ObserverBehaviour property in propertyTargets)
        {
            if (property != null && property.TargetStatus.Status >= Status.TRACKED)
            {
                // Comment: Detection: Overlap Player & Property
                //float distance = Vector3.Distance(transform.position, property.transform.position);

                // Comment: Detection: 0.3f on "Y-axis" above overlap Player & Property
                // Comment: "World Center Mode": "SPECIFIC" (Position used by Unity originally)
                //float distance = Vector3.Distance(transform.position, property.transform.position + new Vector3(0f, 0.3f, 0f));

                // Comment: Detection: 0.3f on "Z-axis" above overlap Player & Property
                // Comment: "World Center Mode": "DEVICE" (Postion used by Vuforia AR Camera)
                float distance = Vector3.Distance(transform.position, property.transform.position + new Vector3(0f, 0f, 0.3f));

                PlayerLandedPropertyUIText += $"Distance{distanceNum + 1}: {distance:F3}";

                if (distance < triggerDistance)
                {
                    TriggerPropertyEffect(property);
                    PlayerLandedPropertyUIText += $"(Landed)";
                }

                PlayerLandedPropertyUIText += $"\n";
            }

            distanceNum++;
        }

        PlayerLandedPropertyUI.Instance.UpdateUI(PlayerLandedPropertyUIText);
    }

    void TriggerPropertyEffect(ObserverBehaviour property)
    {
        HideAllAnimals();

        foreach (Transform child in property.transform)
        {
            if (child.name.EndsWith("Model"))
            {
                child.gameObject.SetActive(true);
            }
        }

        Debug.Log("Landed on: " + property.TargetName);
    }
    void HideAllAnimals()
    {
        foreach (ObserverBehaviour property in propertyTargets)
        {
            foreach (Transform child in property.transform)
            {
                if (child.name.EndsWith("Model"))
                {
                    child.gameObject.SetActive(false);
                }
            }
        }
    }
}