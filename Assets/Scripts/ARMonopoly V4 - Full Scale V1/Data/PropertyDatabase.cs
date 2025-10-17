using System.Collections.Generic;
using UnityEngine;

namespace ARMonopoly_V4___Full_Scale_V1.Data
{
    [CreateAssetMenu(menuName="ARMonopolyV4-FullScale/Property DB")]
    public class PropertyDatabase : ScriptableObject
    {
        public List<PropertyDef> properties = new();
        Dictionary<string, PropertyDef> _map;

        void OnEnable() {
            _map = new();
            foreach (var property in properties)
            {
                if (property && !string.IsNullOrEmpty(property.id))
                    _map[property.id] = property;
            }
        }
        public PropertyDef Get(string id) => (_map != null && _map.TryGetValue(id, out var d)) ? d : null;
    }
}