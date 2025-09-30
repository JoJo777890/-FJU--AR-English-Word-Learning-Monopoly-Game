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
        public float dwellSeconds = 0.35f;
        public float scaleUpFactor = 1.3f;
        public string contentSuffix = "Plane";

        private ObserverBehaviour playerObserver;
        private PlayerTag playerTag;

        private readonly Dictionary<Transform, Vector3> baseScale = new();
        private readonly Dictionary<ObserverBehaviour, float> dwell = new();

        void Start()
        {
            playerObserver = GetComponent<ObserverBehaviour>();
            playerTag      = GetComponent<PlayerTag>();
            if (playerTag != null) SimpleGame.RegisterPlayer(playerTag);

            foreach (var prop in propertyTargets)
            {
                if (prop == null) continue;
                foreach (Transform child in prop.transform)
                {
                    if (child.name.EndsWith(contentSuffix) && !baseScale.ContainsKey(child))
                        baseScale[child] = child.localScale;
                }
                dwell[prop] = 0f;
            }
        }

        void Update()
        {
            if (playerObserver == null || playerObserver.TargetStatus.Status < Status.TRACKED) return;
            if (playerTag == null) return;

            string distancesUI = "";

            foreach (var prop in propertyTargets)
            {
                if (prop == null || prop.TargetStatus.Status < Status.TRACKED) continue;

                float d = Vector3.Distance(transform.position, prop.transform.position);
                bool close = d < triggerDistance;
                distancesUI += $"{prop.TargetName.Substring(0, 10)} d={d:F3}{(close ? " (close)" : "")}\n";

                // scale only content children
                foreach (Transform child in prop.transform)
                {
                    if (!child.name.EndsWith(contentSuffix)) continue;
                    var orig = baseScale.TryGetValue(child, out var s) ? s : child.localScale;
                    child.localScale = close ? orig * scaleUpFactor : orig;
                    if (!child.gameObject.activeSelf) child.gameObject.SetActive(true);
                }

                // dwell (stable landing)
                if (close) dwell[prop] += Time.deltaTime; else dwell[prop] = 0f;

                if (close && dwell[prop] >= dwellSeconds)
                {
                    dwell[prop] = -999f; // debounce until we leave
                    var tag = prop.GetComponent<PropertyTag>();
                    if (tag != null)
                    {
                        GameEvents.RaisePropertyLanded(new PropertyLanded{
                            playerId = playerTag.playerId,
                            propertyId = tag.propertyId,
                            propertyName = tag.displayName
                        });
                    }
                }
                else if (!close && dwell[prop] < 0f)
                {
                    dwell[prop] = 0f; // reset to allow next trigger
                }
            }

            GameEvents.RaiseDistancesUpdated(distancesUI);
        }
    }
}
