using System.Collections.Generic;
using UnityEngine;

namespace ARMonopoly_V5___Full_Scale_V2.Data
{
    /// <summary>
    /// A database of all properties.
    /// Create one from 'Assets > Create > AR Monopoly > Property Database'.
    /// </summary>
    [CreateAssetMenu(menuName = "ARMonopoly_V5___Full_Scale_V2/Property Database")]
    public class PropertyDatabase : ScriptableObject
    {
        public List<PropertyDef> AllProperties;

        private Dictionary<string, PropertyDef> _propertyMap;

        public PropertyDef GetProperty(string propertyID)
        {
            if (_propertyMap == null)
                InitializeMap();

            _propertyMap.TryGetValue(propertyID, out PropertyDef def);
            return def;
        }

        private void InitializeMap()
        {
            _propertyMap = new Dictionary<string, PropertyDef>();
            if (AllProperties == null) return;

            foreach (var prop in AllProperties)
            {
                if (prop != null && !string.IsNullOrEmpty(prop.PropertyID))
                {
                    _propertyMap[prop.PropertyID] = prop;
                }
            }
        }
    }
}