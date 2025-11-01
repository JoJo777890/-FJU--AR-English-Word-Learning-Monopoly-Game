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
    /// Does NOT control scaling.
    /// </summary>
    [RequireComponent(typeof(PlayerTag), typeof(ObserverBehaviour))]
    public class PlayerTokenTrigger : MonoBehaviour
    {
        private PlayerTag _playerTag;
        private ObserverBehaviour _playerObserver;
        private GameConfig _config;

        // Tracks which property this player is currently "in"
        private string _currentProximityID = null;

        void Start()
        {
            _playerTag = GetComponent<PlayerTag>();
            _playerObserver = GetComponent<ObserverBehaviour>();
            
            if (AppGame.Instance == null)
            {
                Debug.LogError($"PlayerTokenTrigger (P{_playerTag.PlayerID}): AppGame.Instance is not ready!");
                return;
            }
            _config = AppGame.Instance.Config;

            if (_config == null)
                Debug.LogError($"PlayerTokenTrigger (P{_playerTag.PlayerID}): GameConfig not found!");
        }

        void Update()
        {
            if (_playerObserver == null || _config == null || _playerTag == null) return;

            // Only check if this player's token is being tracked
            if (_playerObserver.TargetStatus.Status < Status.TRACKED)
            {
                // If we were near a property and lost tracking, fire an exit event
                HandleProximityExit(null); // Pass null to signify an "exit all"
                return;
            }

            PropertyTag closestProp = null;
            float closestDist = float.MaxValue;

            // Find the closest *tracked* property
            foreach (var prop in PropertyVisuals.AllTrackedProperties)
            {
                if (prop == null) continue; // Safety check
                
                float dist = Vector3.Distance(transform.position, prop.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestProp = prop;
                }
            }

            // Check if the closest property is within range
            if (closestProp != null && closestDist < _config.TriggerDistance)
            {
                // We are close to a property
                if (_currentProximityID != closestProp.PropertyID) // <-- FIXED
                {
                    // This is a NEW property, fire exit for the old and enter for the new
                    HandleProximityExit(_currentProximityID);
                    HandleProximityEnter(closestProp);
                }
            }
            else
            {
                // We are not close to any property
                HandleProximityExit(_currentProximityID);
            }
        }

        private void HandleProximityEnter(PropertyTag prop)
        {
            _currentProximityID = prop.PropertyID; // <-- FIXED
            GameEvents.RaiseProximityEnter(new ProximityPayload
            {
                PlayerID = _playerTag.PlayerID,
                PropertyID = prop.PropertyID // <-- FIXED
            });
        }

        private void HandleProximityExit(string oldPropertyID)
        {
            if (string.IsNullOrEmpty(oldPropertyID)) return;
            
            _currentProximityID = null;
            GameEvents.RaiseProximityExit(new ProximityPayload
            {
                PlayerID = _playerTag.PlayerID,
                PropertyID = oldPropertyID
            });
        }
    }
}

