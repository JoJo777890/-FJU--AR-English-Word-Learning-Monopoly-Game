using System.Collections.Generic;
using ARMonopoly_V5___Full_Scale_V2.Core;
using ARMonopoly_V5___Full_Scale_V2.Data;
using UnityEngine;

namespace ARMonopoly_V5___Full_Scale_V2.Property
{
    /// <summary>
    /// Attached to a Property's Image Target.
    /// Listens for proximity events and scales its content.
    /// </summary>
    [RequireComponent(typeof(PropertyTag))]
    public class PropertyVisuals : MonoBehaviour
    {
        private GameConfig _config;
        private string _propertyID;
        
        // Tracks which players are close
        private HashSet<int> _playersNearby = new HashSet<int>();
        
        // Caches the original scales of child content
        private Dictionary<Transform, Vector3> _baseScales = new Dictionary<Transform, Vector3>();
        private List<Transform> _contentChildren = new List<Transform>();

        void Awake()
        {
            _config = AppGame.Instance.Config;
            _propertyID = GetComponent<PropertyTag>().PropertyID;
            
            // Find all content children and cache their base scales
            foreach (Transform child in transform)
            {
                if (child.name.EndsWith(_config.PropertyContentSuffix))
                {
                    _contentChildren.Add(child);
                    _baseScales[child] = child.localScale;
                }
            }
        }

        private void OnEnable()
        {
            GameEvents.OnProximityEnter += HandleProximityEnter;
            GameEvents.OnProximityExit += HandleProximityExit;
        }

        private void OnDisable()
        {
            GameEvents.OnProximityEnter -= HandleProximityEnter;
            GameEvents.OnProximityExit -= HandleProximityExit;
        }

        private void HandleProximityEnter(ProximityPayload payload)
        {
            // Is this event for *this* property?
            if (payload.PropertyID != _propertyID) return;

            // Track the player
            _playersNearby.Add(payload.PlayerID);

            // Scale up (if not already)
            if (_playersNearby.Count > 0)
            {
                SetScale(_config.PropertyScaleUpFactor);
            }
        }

        private void HandleProximityExit(ProximityPayload payload)
        {
            // Is this event for *this* property?
            if (payload.PropertyID != _propertyID) return;

            // Remove the player
            _playersNearby.Remove(payload.PlayerID);

            // Scale down ONLY if *no one* is nearby
            if (_playersNearby.Count == 0)
            {
                SetScale(1.0f);
            }
        }

        private void SetScale(float scaleFactor)
        {
            foreach (var child in _contentChildren)
            {
                if (_baseScales.TryGetValue(child, out var baseScale))
                {
                    child.localScale = baseScale * scaleFactor;
                }
            }
        }
    }
}

