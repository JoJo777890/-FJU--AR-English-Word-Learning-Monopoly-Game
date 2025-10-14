using System.Collections.Generic;
using ARMonopoly_Medium_Scale.Board;
using ARMonopoly_Medium_Scale.Core;
using ARMonopoly_Medium_Scale.Players;
using ARMonopoly_Medium_Scale.Services;
using UnityEngine;
using Vuforia;

namespace ARMonopoly_Medium_Scale.AR
{
    public class PlayerTokenTriggerV3 : MonoBehaviour
    {
        public List<ObserverBehaviour> propertyTargets;

        // If AppGame has GameConfig, we read from it; otherwise use these as fallback:
        public float triggerDistance = 0.07f;
        public float dwellSeconds = 0.35f;
        public float scaleUpFactor = 1.3f;
        public string contentSuffix = "Plane";

        private PlayerTag _playerTag;
        private ObserverBehaviour _playerObs;
        private AppGame _app;

        private readonly Dictionary<Transform, Vector3> _base = new();
        private readonly Dictionary<ObserverBehaviour, float> _dwell = new();
        
        
        void Start()
        {
            _playerTag = GetComponent<PlayerTag>();
            _playerObs = GetComponent<ObserverBehaviour>();
            _app = FindObjectOfType<AppGame>();

            // read GameConfig
            var cfg = _app ? _app.gameConfig : null;
            if (cfg) {
                triggerDistance = cfg.triggerDistance;
                dwellSeconds = cfg.dwellSeconds;
                scaleUpFactor = cfg.scaleUpFactor;
                contentSuffix = cfg.contentSuffix;
            }

            // cache scales
            foreach (var prop in propertyTargets)
            {
                if (!prop) 
                    continue;
                foreach (Transform c in prop.transform)
                    if (c.name.EndsWith(contentSuffix) && !_base.ContainsKey(c))
                    {
                        _base[c] = c.localScale;
                    }
                _dwell[prop] = 0f;
            }

            // register player money in EconomyService
            if (_app) 
                _app.GetService<EconomyService>()?.RegisterPlayer(_playerTag.playerId, _playerTag.money);
        }

        void Update()
        {
            if (!_playerObs || _playerObs.TargetStatus.Status < Status.TRACKED) 
                return;
            if (_playerTag == null) 
                return;

            string debug = "";

            foreach (var prop in propertyTargets)
            {
                if (!prop || prop.TargetStatus.Status < Status.TRACKED) 
                    continue;

                float d = Vector3.Distance(transform.position, prop.transform.position);
                bool close = d < triggerDistance;
                debug += $"{prop.TargetName.Substring(0, 10)} distance={d:F3}{(close ? " (close)" : "")}\n"; // ".Substring(0, 10)" is for e.g., "Location_A---Patterned" with only the "Location_A".

                foreach (Transform c in prop.transform)
                {
                    if (!c.name.EndsWith(contentSuffix)) 
                        continue;
                    var orig = _base.TryGetValue(c, out var s) ? s : c.localScale;
                    c.localScale = close ? orig * scaleUpFactor : orig;
                    if (!c.gameObject.activeSelf)
                    {
                        c.gameObject.SetActive(true);
                    }
                }

                if (close)
                {
                    _dwell[prop] += Time.deltaTime;
                }
                else
                {
                    _dwell[prop] = 0f;
                }

                if (close && _dwell[prop] >= dwellSeconds)
                {
                    _dwell[prop] = -999f; // prevent repeat until we leave
                    
                    //-- Debug Below --
                    var tag = prop.GetComponent<PropertyTag>();
                    if (tag == null)
                    {
                        Debug.LogError($"[Trigger] PropertyLanded but PropertyTag missing on target '{prop.TargetName}'. Add PropertyTag.");
                    }
                    else
                    {
                        var id = tag.Id; // def.id or fallback gameObject.name
                        if (string.IsNullOrEmpty(id))
                        {
                            Debug.LogError($"[Trigger] PropertyTag.Id is empty on '{prop.TargetName}'. Set PropertyDef.id or rename object.");
                        }
                        else
                        {
                            Debug.Log($"[Trigger] Landed: P{_playerTag.playerId} on {id} ({tag.DisplayName})");
                            GameEvents.RaisePropertyLanded(new PropertyLanded{
                                playerId    = _playerTag.playerId,
                                propertyId  = id,
                                propertyName= tag.DisplayName
                            });
                        }
                    }
                    //-- Debug Above --
                }
                else if (!close && _dwell[prop] < 0f)
                {
                    _dwell[prop] = 0f;
                }
            }

            GameEvents.RaiseDistancesUpdated(debug);
        }
    }
}
