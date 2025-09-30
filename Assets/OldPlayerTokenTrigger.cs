using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class OldPlayerTokenTrigger : MonoBehaviour
{
    public List<ObserverBehaviour> propertyTargets;
    public float triggerDistance = 0.07f;
    public float scaleUpFactor = 1.3f;      // how much bigger when close
    public string contentSuffix = "Plane";  // only scale children whose names end with this

    private ObserverBehaviour playerObserver;
    private string PlayerLandedPropertyUIText;

    // original scales per content child
    private readonly Dictionary<Transform, Vector3> baseScale = new();

    void Start()
    {
        playerObserver = GetComponent<ObserverBehaviour>();

        // cache original scales of content children
        foreach (var prop in propertyTargets)
        {
            if (prop == null) continue;
            foreach (Transform child in prop.transform)
            {
                if (child.name.EndsWith(contentSuffix))
                {
                    if (!baseScale.ContainsKey(child))
                        baseScale[child] = child.localScale;
                }
            }
        }
    }

    void Update()
    {
        PlayerLandedPropertyUIText = "";

        if (playerObserver == null || playerObserver.TargetStatus.Status < Status.TRACKED)
            return;

        int i = 0;
        foreach (var prop in propertyTargets)
        {
            if (prop == null || prop.TargetStatus.Status < Status.TRACKED) { i++; continue; }

            // simple distance (no artificial offset)
            float distance = Vector3.Distance(transform.position, prop.transform.position);
            bool isClose = distance < triggerDistance;

            PlayerLandedPropertyUIText += $"Distance{i + 1}: {distance:F3}" + (isClose ? " (Landed)\n" : "\n");

            // scale ONLY the content child(ren), not the ImageTarget root
            foreach (Transform child in prop.transform)
            {
                if (!child.name.EndsWith(contentSuffix)) continue;

                Vector3 orig = baseScale.TryGetValue(child, out var s) ? s : child.localScale;
                child.localScale = isClose ? orig * scaleUpFactor : orig;

                if (!child.gameObject.activeSelf) child.gameObject.SetActive(true);
            }

            i++;
        }

        PlayerLandedPropertyUI.Instance.UpdateUI(PlayerLandedPropertyUIText);
    }
}
