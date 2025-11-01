using System.Collections.Generic;
using ARMonopoly_V5___Full_Scale_V2.Core;
using ARMonopoly_V5___Full_Scale_V2.Data;
using UnityEngine;
using Vuforia;

namespace ARMonopoly_V5___Full_Scale_V2.Property
{
    /// <summary>
    /// Attached to the Property's Image Target.
    /// Manages its own scale based on proximity events.
    /// Also reports when it is being tracked.
    /// </summary>
    [RequireComponent(typeof(PropertyTag), typeof(ObserverBehaviour))]
    public class PropertyVisuals : MonoBehaviour
    {
        // A static list of all properties currently being tracked by Vuforia
        public static HashSet<PropertyTag> AllTrackedProperties = new HashSet<PropertyTag>();

        private PropertyTag _propertyTag;
        private ObserverBehaviour _propertyObserver;
        private GameConfig _config;
        private Transform _contentChild;
        private Vector3 _baseScale;
        private HashSet<int> _playersInProximity = new HashSet<int>();

        void Awake()
        {
            _propertyTag = GetComponent<PropertyTag>();
            _propertyObserver = GetComponent<ObserverBehaviour>();
        }

        void Start()
        {
            _config = AppGame.Instance.Config;
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
        }

        private void OnEnable()
        {
            GameEvents.OnProximityEnter += HandleProximityEnter;
            GameEvents.OnProximityExit += HandleProximityExit;
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

        private void HandleProximityEnter(ProximityPayload payload)
        {
            // Is this event about *this* property?
            if (payload.PropertyID == _propertyTag.PropertyID)
            {
                _playersInProximity.Add(payload.PlayerID);
                UpdateScale();
            }
        }

        private void HandleProximityExit(ProximityPayload payload)
        {
            // Is this event about *this* property?
            if (payload.PropertyID == _propertyTag.PropertyID)
            {
                _playersInProximity.Remove(payload.PlayerID);
                UpdateScale();
            }
        }

        private void UpdateScale()
        {
            if (_contentChild == null) return;

            // If *any* player is nearby, scale up.
            if (_playersInProximity.Count > 0)
            {
                _contentChild.localScale = _baseScale * _config.ScaleUpFactor;
            }
            else
            {
                _contentChild.localScale = _baseScale;
            }
        }
    }
}

