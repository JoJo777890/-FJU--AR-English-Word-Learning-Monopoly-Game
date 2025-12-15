using System.Collections.Generic;
using ARMonopoly_V5___Full_Scale_V2.Core;
using ARMonopoly_V5___Full_Scale_V2.Data;
using UnityEngine;
using Vuforia;

namespace ARMonopoly_V5___Full_Scale_V2.Property
{
    /// <summary>
    /// Handles the visual state (scaling) of a property based on player proximity.
    /// Also maintains the static list of all currently tracked properties.
    /// Attached to the Property's Image Target.
    /// </summary>
    public class PropertyVisuals : MonoBehaviour
    {
        /// <summary>
        /// Static list of all *currently tracked* properties. 
        /// PlayerTokenTrigger uses this for efficient proximity checks.
        /// </summary>
        public static HashSet<PropertyTag> AllTrackedProperties = new HashSet<PropertyTag>();

        private PropertyTag _propertyTag;
        private ObserverBehaviour _propertyObserver;
        private GameConfig _config;
        private Transform _contentChild; // The 3D model/plane to scale
        private Vector3 _baseScale;
        
        // Tracks which players are currently near *this* property
        private HashSet<int> _playersInProximity = new HashSet<int>();
        
        private bool _isInitialized = false;

        void Awake()
        {
            _propertyTag = GetComponent<PropertyTag>();
            _propertyObserver = GetComponent<ObserverBehaviour>();
        }

        // Called by AppGame
        public void Construct(GameConfig config)
        {
            _config = config;
            if (_config == null)
            {
                Debug.LogError($"PropertyVisuals ({_propertyTag.PropertyID}): GameConfig not found!");
                return;
            }

            // Find the content child to scale
            foreach (Transform child in transform)
            {
                if (child.name.EndsWith(_config.ContentSuffix))
                {
                    _contentChild = child;
                    _baseScale = child.localScale;
                    break;
                }
            }
            if (_contentChild == null)
                Debug.LogWarning($"PropertyVisuals ({_propertyTag.PropertyID}): No child found with suffix '{_config.ContentSuffix}' to scale.");
            
            _isInitialized = true;
        }

        private void OnEnable()
        {
            // Listen for proximity events to update scale
            GameEvents.OnProximityEnter += HandleProximityEnter;
            GameEvents.OnProximityExit += HandleProximityExit;
            // Listen for Vuforia status to update the static list
            _propertyObserver.OnTargetStatusChanged += HandleTargetStatusChanged;
        }

        private void OnDisable()
        {
            GameEvents.OnProximityEnter -= HandleProximityEnter;
            GameEvents.OnProximityExit -= HandleProximityExit;
            _propertyObserver.OnTargetStatusChanged -= HandleTargetStatusChanged;

            // Clean up from static list if disabled/destroyed
            if (_propertyTag != null)
                AllTrackedProperties.Remove(_propertyTag);
        }

        /// <summary>
        /// Updates the static list of all tracked properties.
        /// </summary>
        private void HandleTargetStatusChanged(ObserverBehaviour ob, TargetStatus status)
        {
            bool isTracked = status.Status == Status.TRACKED || status.Status == Status.EXTENDED_TRACKED;

            if (isTracked)
            {
                AllTrackedProperties.Add(_propertyTag);
            }
            else
            {
                AllTrackedProperties.Remove(_propertyTag);
            }
        }

        /// <summary>
        /// A player has entered this property's proximity.
        /// </summary>
        private void HandleProximityEnter(ProximityPayload payload)
        {
            if (payload.PropertyID == _propertyTag.PropertyID)
            {
                _playersInProximity.Add(payload.PlayerID);
                UpdateScale();
            }
        }

        /// <summary>
        /// A player has exited this property's proximity.
        /// </summary>
        private void HandleProximityExit(ProximityPayload payload)
        {
            // Is this event about *this* property?
            if (payload.PropertyID == _propertyTag.PropertyID)
            {
                _playersInProximity.Remove(payload.PlayerID);
                UpdateScale();
            }
        }

        /// <summary>
        /// Updates the scale of the content child based on player proximity.
        /// </summary>
        private void UpdateScale()
        {
            if (!_isInitialized || _contentChild == null) return;

            // If *any* player is nearby, scale up.
            if (_playersInProximity.Count > 0)
                _contentChild.localScale = _baseScale * _config.ScaleUpFactor;
            else
                _contentChild.localScale = _baseScale;
        }
    }
}