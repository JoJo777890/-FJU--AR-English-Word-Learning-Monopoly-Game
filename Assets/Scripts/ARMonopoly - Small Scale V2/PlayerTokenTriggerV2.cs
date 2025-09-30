// Assets/Scripts/Simple/PlayerTokenTrigger.cs
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

namespace ARMonopoly.Simple
{
    public class PlayerTokenTriggerV2 : MonoBehaviour
    {
        [Header("Targets")]
        public List<ObserverBehaviour> propertyTargets;

        [Header("Proximity")]
        public float triggerDistance = 0.07f;
        public float dwellSeconds = 0.35f;         // must stay close this long to "land"
        public float scaleUpFactor = 1.3f;         // how much bigger when close
        public string contentSuffix = "Plane";     // only scale children whose names end with this

        private ObserverBehaviour playerObserver;
        private PlayerTag playerTag;

        // per-property original scales (only for content children)
        private readonly Dictionary<Transform, Vector3> baseScale = new();
        // dwell clocks per property
        private readonly Dictionary<ObserverBehaviour, float> dwellClock = new();

        void Start()
        {
            playerObserver = GetComponent<ObserverBehaviour>();
            playerTag = GetComponent<PlayerTag>();
            if (playerTag != null) SimpleGame.RegisterPlayer(playerTag);

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
                dwellClock[prop] = 0f;
            }
        }

        void Update()
        {
            if (playerObserver == null || playerObserver.TargetStatus.Status < Status.TRACKED) return;
            if (playerTag == null) return;

            string ui = "";

            foreach (var prop in propertyTargets)
            {
                if (prop == null || prop.TargetStatus.Status < Status.TRACKED) continue;

                float distance = Vector3.Distance(transform.position, prop.transform.position);
                bool isClose = distance < triggerDistance;
                ui += $"{prop.TargetName} d={distance:F3}{(isClose ? " (close)" : "")}\n";

                // visual: scale ONLY content child(ren), not ImageTarget root
                foreach (Transform child in prop.transform)
                {
                    if (!child.name.EndsWith(contentSuffix)) continue;
                    Vector3 orig = baseScale.TryGetValue(child, out var s) ? s : child.localScale;
                    child.localScale = isClose ? orig * scaleUpFactor : orig;
                    if (!child.gameObject.activeSelf) child.gameObject.SetActive(true);
                }

                // dwell clock to stabilize "landed"
                if (isClose) dwellClock[prop] += Time.deltaTime;
                else dwellClock[prop] = 0f;

                // landed event once per dwell
                if (isClose && dwellClock[prop] >= dwellSeconds)
                {
                    dwellClock[prop] = -999f; // prevent repeat until we leave

                    var tag = prop.GetComponent<PropertyTag>();
                    if (tag != null)
                    {
                        int currentOwner = SimpleGame.GetOwner(tag.propertyId);
                        if (currentOwner == -1)
                        {
                            // show buy prompt (UI button will call BuyCurrent)
                            SimpleUI.ShowBuy(tag, playerTag, () => SimpleGame.Buy(playerTag, tag));
                        }
                        else
                        {
                            SimpleGame.PayRent(playerTag, tag);
                        }

                        SimpleUI.Log($"{playerTag.playerName} landed on {tag.displayName}.");
                    }
                }
                else if (!isClose && dwellClock[prop] < 0f)
                {
                    // we left; reset so next close will re-trigger
                    dwellClock[prop] = 0f;
                }
            }

            SimpleUI.UpdateDistances(ui);
        }
    }
}
