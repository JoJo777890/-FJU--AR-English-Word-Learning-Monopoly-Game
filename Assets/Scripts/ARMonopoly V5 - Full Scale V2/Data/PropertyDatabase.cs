using System.Collections.Generic;
using UnityEngine;

namespace ARMonopoly_V5___Full_Scale_V2.Data
{
    /// <summary>
    /// ScriptableObject asset that holds all PropertyDef assets for fast lookup.
    /// Create one from 'Assets > Create > ARMonopoly_V5___Full_Scale_V2 > Property Database'.
    /// </summary>
    [CreateAssetMenu(menuName = "ARMonopoly_V5___Full_Scale_V2/Property Database")]
    public class PropertyDatabase : ScriptableObject
    {
        // (Tip): A list of all PropertyDef assets in the game.
        public List<PropertyDef> AllProperties;

        // Internal dictionary for fast O(1) lookups by string ID
        private Dictionary<string, PropertyDef> _propertyMap;

        /// <summary>
        /// Finds a PropertyDef by its unique string ID.
        /// </summary>
        public PropertyDef GetProperty(string propertyID)
        {
            // Lazy initialization for the dictionary
            if (_propertyMap == null)
                InitializeMap();

            _propertyMap.TryGetValue(propertyID, out PropertyDef def);
            return def;
        }

        /// <summary>
        /// Builds the dictionary for fast lookups.
        /// </summary>
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