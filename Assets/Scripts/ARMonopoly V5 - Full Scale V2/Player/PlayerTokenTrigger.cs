using System.Collections.Generic;
using ARMonopoly_V5___Full_Scale_V2.Core;
using ARMonopoly_V5___Full_Scale_V2.Data;
using ARMonopoly_V5___Full_Scale_V2.Property;
using UnityEngine;
using Vuforia;

namespace ARMonopoly_V5___Full_Scale_V2.Player
{
    [RequireComponent(typeof(PlayerTag))]
    public class PlayerTokenTrigger : MonoBehaviour
    {
        public List<PropertyTag> AllProperties = new List<PropertyTag>();

        private PlayerTag _playerTag;
        private ObserverBehaviour _playerObserver;
        private GameConfig _config;

        private Dictionary<string, float> _dwellTimers = new Dictionary<string, float>();
        private HashSet<string> _currentlyInside = new HashSet<string>();

        void Start()
        {
            _playerTag = GetComponent<PlayerTag>();
            _playerObserver = GetComponent<ObserverBehaviour>();
            _config = AppGame.Instance.Config;

            AllProperties.AddRange(FindObjectsOfType<PropertyTag>());
            
            foreach (var prop in AllProperties)
            {
                if(prop == null || string.IsNullOrEmpty(prop.ID)) continue;
                _dwellTimers[prop.ID] = 0f;
            }
        }

        void Update()
        {
            if (_playerObserver == null || _playerObserver.TargetStatus.Status < Status.TRACKED)
            {
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

                // --- NOTE: All scaling logic has been removed ---

                if (isClose)
                {
                    _dwellTimers[propID] += Time.deltaTime;

                    if (!wasInside && _dwellTimers[propID] >= _config.DwellSeconds)
                    {
                        // --- FIRE ENTER EVENT ---
                        _currentlyInside.Add(propID);
                        Debug.Log($"PlayerTokenTrigger (P{_playerTag.PlayerID}): Firing ProximityEnter for {propID}");
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
                    Debug.Log($"PlayerTokenTrigger (P{_playerTag.PlayerID}): Firing ProximityExit for {propID}");
                    GameEvents.RaiseProximityExit(new ProximityPayload
                    {
                        PlayerID = _playerTag.PlayerID,
                        PropertyID = propID,
                        PropertyDef = prop.PropertyDefinition
                    });
                }
            }
        }
    }
}

