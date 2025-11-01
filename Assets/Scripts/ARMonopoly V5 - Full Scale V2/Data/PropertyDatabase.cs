using System.Collections.Generic;
using UnityEngine;

namespace ARMonopoly_V5___Full_Scale_V2.Data
{
    [CreateAssetMenu(menuName = "ARMonopoly_V5___Full_Scale_V2/Property Database")]
    public class PropertyDatabase : ScriptableObject
    {
        public List<PropertyDef> AllProperties;

        private Dictionary<string, PropertyDef> _propertiesByID;

        private void OnEnable()
        {
            // Initialize dictionary for fast lookup
            _propertiesByID = new Dictionary<string, PropertyDef>();
            if (AllProperties == null) return;

            foreach (var prop in AllProperties)
            {
                if (prop != null && !string.IsNullOrEmpty(prop.PropertyID))
                {
                    _propertiesByID[prop.PropertyID] = prop;
                }
            }
        }

        public PropertyDef GetProperty(string id)
        {
            _propertiesByID.TryGetValue(id, out var prop);
            if (prop == null)
            {
                Debug.LogWarning($"PropertyDatabase: No property found with ID '{id}'");
            }
            return prop;
        }
    }
}