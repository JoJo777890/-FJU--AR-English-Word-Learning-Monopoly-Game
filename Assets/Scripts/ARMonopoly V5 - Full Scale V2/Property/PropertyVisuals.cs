using System.Collections.Generic;
using ARMonopoly_V5___Full_Scale_V2.Core;
using ARMonopoly_V5___Full_Scale_V2.Data;
using UnityEngine;

namespace ARMonopoly_V5___Full_Scale_V2.Property
{
    /// <summary>
    /// Attach to each Property ImageTarget.
    /// Listens for proximity events and scales its own content.
    /// This fixes the race condition.
    /// </summary>
    public class PropertyVisuals : MonoBehaviour
    {
        private GameConfig _config;
        private Dictionary<Transform, Vector3> _baseScales = new Dictionary<Transform, Vector3>();
        
        // This set tracks which players are currently "inside" this property's radius
        private HashSet<int> _playersInside = new HashSet<int>();

        void Start()
        {
            _config = AppGame.Instance.Config;
            if (_config == null)
            {
                Debug.LogError($"PropertyVisuals on {gameObject.name}: Failed to get GameConfig.");
                return;
            }

            // Cache the base scale of all content children
            foreach (Transform child in transform)
            {
                if (child.name.EndsWith(_config.ContentSuffix))
                {
                    if (!_baseScales.ContainsKey(child))
                    {
                        _baseScales.Add(child, child.localScale);
                    }
                }
            }
        }

        void OnEnable()
        {
            GameEvents.OnProximityEnter += HandleProximityEnter;
            GameEvents.OnProximityExit += HandleProximityExit;
        }

        void OnDisable()
        {
            GameEvents.OnProximityEnter -= HandleProximityEnter;
            GameEvents.OnProximityExit -= HandleProximityExit;
        }

        private void HandleProximityEnter(ProximityPayload payload)
        {
            // Check if this event is for ME
            if (payload.PropertyID != GetComponent<PropertyTag>().ID)
            {
                return;
            }

            // A player entered my radius
            _playersInside.Add(payload.PlayerID);
            UpdateScale();
        }

        private void HandleProximityExit(ProximityPayload payload)
        {
            // Check if this event is for ME
            if (payload.PropertyID != GetComponent<PropertyTag>().ID)
            {
                return;
            }

            // A player left my radius
            _playersInside.Remove(payload.PlayerID);
            UpdateScale();
        }

        private void UpdateScale()
        {
            // If *any* player is inside, scale up. Otherwise, scale down.
            bool isAnyPlayerClose = _playersInside.Count > 0;
            float scaleFactor = isAnyPlayerClose ? _config.ScaleUpFactor : 1.0f;

            foreach (var pair in _baseScales)
            {
                Transform child = pair.Key;
                Vector3 baseScale = pair.Value;
                child.localScale = baseScale * scaleFactor;
            }
            
            Debug.Log($"PropertyVisuals on {gameObject.name}: Updating scale. Players inside: {_playersInside.Count}. ScaleFactor: {scaleFactor}");
        }
    }
}
