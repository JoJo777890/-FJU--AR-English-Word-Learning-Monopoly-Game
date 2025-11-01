using System.Collections.Generic;
using ARMonopoly_V5___Full_Scale_V2.Core;
using ARMonopoly_V5___Full_Scale_V2.Data;
using ARMonopoly_V5___Full_Scale_V2.Property;
using UnityEngine;
using Vuforia;

namespace ARMonopoly_V5___Full_Scale_V2.Player
{
    /// <summary>
    /// Attached to the Player's Image Target.
    /// Detects proximity to properties and fires events.
    /// </summary>
    [RequireComponent(typeof(PlayerTag), typeof(ObserverBehaviour))]
    public class PlayerTokenTrigger : MonoBehaviour
    {
        public List<PropertyTag> AllProperties = new List<PropertyTag>();

        private PlayerTag _playerTag;
        private ObserverBehaviour _playerObserver;
        private GameConfig _config;

        // Runtime dictionary to track which properties we are "inside"
        private Dictionary<string, bool> _isCloseTo = new Dictionary<string, bool>();

        void Start()
        {
            _playerTag = GetComponent<PlayerTag>();
            _playerObserver = GetComponent<ObserverBehaviour>();
            _config = AppGame.Instance.Config;

            // Find all properties in the scene
            AllProperties.AddRange(FindObjectsOfType<PropertyTag>());
            
            // Initialize proximity tracking
            foreach (var prop in AllProperties)
            {
                if (prop != null && !string.IsNullOrEmpty(prop.PropertyID))
                {
                    _isCloseTo[prop.PropertyID] = false;
                }
            }
        }

        void Update()
        {
            if (_playerObserver == null || _config == null) return;

            // Only run proximity checks if this player's target is being tracked
            if (_playerObserver.TargetStatus.Status < Status.TRACKED)
            {
                return;
            }

            foreach (var prop in AllProperties)
            {
                if (prop == null || prop.gameObject == null) continue;

                float distance = Vector3.Distance(transform.position, prop.transform.position);
                bool isClose = distance < _config.ProximityTriggerDistance;
                string propID = prop.PropertyID;

                // Use Hysteresis (Enter/Exit)
                bool wasClose = _isCloseTo[propID];

                if (isClose && !wasClose)
                {
                    // --- ENTER ---
                    _isCloseTo[propID] = true;
                    GameEvents.RaiseProximityEnter(new ProximityPayload
                    {
                        PlayerID = _playerTag.PlayerID,
                        PropertyID = propID
                    });
                }
                else if (!isClose && wasClose)
                {
                    // --- EXIT ---
                    _isCloseTo[propID] = false;
                    GameEvents.RaiseProximityExit(new ProximityPayload
                    {
                        PlayerID = _playerTag.PlayerID,
                        PropertyID = propID
                    });
                }
            }
        }
    }
}

