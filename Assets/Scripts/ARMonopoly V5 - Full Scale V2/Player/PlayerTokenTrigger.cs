using ARMonopoly_V5___Full_Scale_V2.Core;
using ARMonopoly_V5___Full_Scale_V2.Data;
using ARMonopoly_V5___Full_Scale_V2.Property;
using UnityEngine;
using Vuforia;

namespace ARMonopoly_V5___Full_Scale_V2.Player
{
    /// <summary>
    /// Handles AR proximity detection for this player token.
    /// Fires proximity events, but does not control visuals.
    /// Attached to the Player's Image Target.
    /// </summary>
    [RequireComponent(typeof(PlayerTag), typeof(ObserverBehaviour))]
    public class PlayerTokenTrigger : MonoBehaviour
    {
        private PlayerTag _playerTag;
        private ObserverBehaviour _playerObserver;
        private GameConfig _config;

        // Tracks which property this player is currently "in" to avoid firing events every frame.
        private string _currentProximityID = null;

        /// <summary>
        /// Caches required components and GameConfig.
        /// </summary>
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

        /// <summary>
        /// Finds the closest tracked property and checks if it's within trigger distance.
        /// </summary>
        void Update()
        {
            if (_playerObserver == null || _config == null || _playerTag == null) return;

            // Only check if this player's token is being tracked
            if (_playerObserver.TargetStatus.Status < Status.TRACKED)
            {
                // If we were near a property and lost tracking, fire an exit event
                HandleProximityExit(_currentProximityID);
                return;
            }

            PropertyTag closestProp = null;
            float closestDist = float.MaxValue;

            // Find the closest *tracked* property from the static list in PropertyVisuals
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
                // We are close to a property.
                if (_currentProximityID != closestProp.PropertyID)
                {
                    // This is a NEW property, fire exit for the old and enter for the new
                    HandleProximityExit(_currentProximityID);
                    HandleProximityEnter(closestProp);
                }
            }
            else
            {
                // We are not close to any property, fire an exit for the current one
                HandleProximityExit(_currentProximityID);
            }
        }

        /// <summary>
        /// Fires the OnProximityEnter event and updates the current proximity ID.
        /// </summary>
        private void HandleProximityEnter(PropertyTag prop)
        {
            _currentProximityID = prop.PropertyID;
            GameEvents.RaiseProximityEnter(new ProximityPayload
            {
                PlayerID = _playerTag.PlayerID,
                PropertyID = prop.PropertyID
            });
        }

        /// <summary>
        /// Fires the OnProximityExit event and clears the current proximity ID.
        /// </summary>
        private void HandleProximityExit(string oldPropertyID)
        {
            // Only fire an exit event if we were actually *in* a proximity zone
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