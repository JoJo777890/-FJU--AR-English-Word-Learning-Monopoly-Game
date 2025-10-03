using System.Collections.Generic;
using UnityEngine;
using Vuforia;

namespace ARMonopoly_Medium_Scale
{
    public class PlayerTokenTrigger : MonoBehaviour
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
                debug += $"{prop.TargetName} d={d:F3}{(close ? " (close)" : "")}\n";

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
                    _dwell[prop] = -999f;
                    var tag = prop.GetComponent<PropertyTag>();
                    if (tag)
                    {
                        GameEvents.RaisePropertyLanded(new PropertyLanded{
                            playerId = _playerTag.playerId,
                            propertyId = tag.Id,
                            propertyName = tag.DisplayName
                        });
                    }
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
