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
        // **FIXED: Now gets list from AppGame, no FindObjectsOfType**
        private List<PropertyTag> _allProperties;

        private PlayerTag _playerTag;
        private ObserverBehaviour _playerObserver;
        private GameConfig _config;

        // Runtime dictionary to track which properties we are "inside"
        private Dictionary<string, bool> _isCloseTo = new Dictionary<string, bool>();

        void Start()
        {
            _playerTag = GetComponent<PlayerTag>();
            _playerObserver = GetComponent<ObserverBehaviour>();

            // **FIXED: Get config and property list from AppGame**
            if (AppGame.Instance == null)
            {
                Debug.LogError($"PlayerTokenTrigger (P{_playerTag.PlayerID}): AppGame.Instance is null!");
                return;
            }
            _config = AppGame.Instance.Config;
            
            _allProperties = AppGame.Instance.AllSceneProperties;
            if (_allProperties == null || _allProperties.Count == 0)
            {
                Debug.LogError($"PlayerTokenTrigger (P{_playerTag.PlayerID}): Could not get property list from AppGame. Is AppGame's list populated by PropertyTags?");
                return;
            }
            
            // Initialize proximity tracking
            foreach (var prop in _allProperties)
            {
                if (prop != null && !string.IsNullOrEmpty(prop.PropertyID))
                {
                    _isCloseTo[prop.PropertyID] = false;
                }
            }
        }

        void Update()
        {
            if (_playerObserver == null || _config == null || _allProperties == null) return;

            // Only run proximity checks if this player's target is being tracked
            if (_playerObserver.TargetStatus.Status < Status.TRACKED)
            {
                return;
            }

            foreach (var prop in _allProperties)
            {
                if (prop == null || prop.gameObject == null) continue;

                // **FIXED: Added checks for safety**
                string propID = prop.PropertyID;
                if (string.IsNullOrEmpty(propID) || !_isCloseTo.ContainsKey(propID))
                {
                    // This can happen if a PropertyTag hasn't registered yet or has no PropertyDef
                    continue; 
                }

                float distance = Vector3.Distance(transform.position, prop.transform.position);
                bool isClose = distance < _config.ProximityTriggerDistance;
                
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

