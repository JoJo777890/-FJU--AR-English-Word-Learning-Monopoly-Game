using System.Collections.Generic;
using UnityEngine;

namespace ARMonopoly_V4___Full_Scale_V1.Board
{
    [CreateAssetMenu(menuName="ARMonopolyV4-FullScale/Property DB")]
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