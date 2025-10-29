using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ARMonopoly_V5___Full_Scale_V2.Data
{
    /// <summary>
    /// Holds a list of all properties in the game.
    /// This effectively replaces the "BoardDefinition" in an all-ImageTarget game.
    /// </summary>
    [CreateAssetMenu(menuName = "ARMonopoly_V5___Full_Scale_V2/Property Database")]
    public class PropertyDatabase : ScriptableObject
    {
        public List<PropertyDef> AllProperties;

        private Dictionary<string, PropertyDef> _lookup;

        public PropertyDef GetProperty(string propertyID)
        {
            if (_lookup == null)
            {
                _lookup = AllProperties.ToDictionary(p => p.PropertyID);
            }
            _lookup.TryGetValue(propertyID, out var def);
            return def;
        }
    }
}