using System.Collections.Generic;
using UnityEngine;

namespace ARMonopoly_Medium_Scale
{
    [CreateAssetMenu(menuName="ARMonopoly/Property DB")]
    public class PropertyDatabase : ScriptableObject
    {
        public List<PropertyDef> properties = new();
        Dictionary<string, PropertyDef> _map;

        void OnEnable() {
            _map = new();
            foreach (var p in properties)
            {
                if (p && !string.IsNullOrEmpty(p.id))
                    _map[p.id] = p;
            }
        }
        public PropertyDef Get(string id) => (_map != null && _map.TryGetValue(id, out var d)) ? d : null;
    }
}