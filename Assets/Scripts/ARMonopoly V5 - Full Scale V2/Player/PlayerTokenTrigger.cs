using System.Collections.Generic;
using ARMonopoly_V5___Full_Scale_V2.Core;
using ARMonopoly_V5___Full_Scale_V2.Data;
using ARMonopoly_V5___Full_Scale_V2.Property;
using UnityEngine;
using Vuforia;

namespace ARMonopoly_V5___Full_Scale_V2.Player
{
    /// <summary>
    /// Attach to each Player ImageTarget.
    /// Detects proximity to Property ImageTargets and fires events.
    /// </summary>
    [RequireComponent(typeof(PlayerTag))]
    public class PlayerTokenTrigger : MonoBehaviour
    {
        [Header("Runtime")]
        public List<PropertyTag> AllProperties; // Populated by AppGame or at Start

        private PlayerTag _playerTag;
        private ObserverBehaviour _playerObserver;
        private GameConfig _config;

        // Proximity state
        private Dictionary<string, float> _dwellTimers = new Dictionary<string, float>();
        private HashSet<string> _currentlyInside = new HashSet<string>();

        void Start()
        {
            _playerTag = GetComponent<PlayerTag>();
            _playerObserver = GetComponent<ObserverBehaviour>();
            _config = AppGame.Instance.Config;

            // Find all properties in the scene
            AllProperties.AddRange(FindObjectsOfType<PropertyTag>());
            foreach (var prop in AllProperties)
            {
                _dwellTimers[prop.ID] = 0f;
            }
        }

        void Update()
        {
            if (_playerObserver == null || _playerObserver.TargetStatus.Status < Status.TRACKED)
            {
                // Optional: You could raise exit events for all _currentlyInside properties if you lose tracking
                return;
            }

            foreach (var prop in AllProperties)
            {
                var propObserver = prop.GetComponent<ObserverBehaviour>();
                if (propObserver == null || propObserver.TargetStatus.Status < Status.TRACKED)
                {
                    continue;
                }

                float distance = Vector3.Distance(transform.position, prop.transform.position);
                bool isClose = distance < _config.TriggerDistance;
                string propID = prop.ID;
                bool wasInside = _currentlyInside.Contains(propID);

                // Scale visual content (your original logic)
                ScaleContent(prop.transform, isClose);

                if (isClose)
                {
                    _dwellTimers[propID] += Time.deltaTime;

                    if (!wasInside && _dwellTimers[propID] >= _config.DwellSeconds)
                    {
                        // --- FIRE ENTER EVENT ---
                        _currentlyInside.Add(propID);
                        GameEvents.RaiseProximityEnter(new ProximityPayload
                        {
                            PlayerID = _playerTag.PlayerID,
                            PropertyID = propID,
                            PropertyDef = prop.PropertyDefinition
                        });
                    }
                }
                else if (wasInside)
                {
                    // --- FIRE EXIT EVENT ---
                    _currentlyInside.Remove(propID);
                    _dwellTimers[propID] = 0f;
                    GameEvents.RaiseProximityExit(new ProximityPayload
                    {
                        PlayerID = _playerTag.PlayerID,
                        PropertyID = propID,
                        PropertyDef = prop.PropertyDefinition
                    });
                }
            }
        }

        void ScaleContent(Transform propTransform, bool isClose)
        {
            foreach (Transform child in propTransform)
            {
                if (child.name.EndsWith(_config.ContentSuffix))
                {
                    // This assumes you cache the base scale somewhere, or just scale from 1.
                    // For simplicity, we'll just set it.
                    float scale = isClose ? _config.ScaleUpFactor : 1.0f;
                    child.localScale = Vector3.one * scale;
                }
            }
        }
    }
}
