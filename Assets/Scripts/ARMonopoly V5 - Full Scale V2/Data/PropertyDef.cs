using UnityEngine;

namespace ARMonopoly_V5___Full_Scale_V2.Data
{
    /// <summary>
    /// ScriptableObject defining a single property's static data.
    /// Create one from 'Assets > Create > ARMonopoly_V5___Full_Scale_V2 > Property Definition'.
    /// </summary>
    [CreateAssetMenu(menuName = "ARMonopoly_V5___Full_Scale_V2/Property Definition")]
    public class PropertyDef : ScriptableObject
    {
        // (Tip): Unique ID (e.g., 'BOARDWALK') used for lookups.
        public string PropertyID;
        
        /// <summary>The display name for the UI.</summary>
        public string DisplayName;
        
        /// <summary>The purchase price of the property.</summary>
        public int Price;
        
        /// <summary>The base rent when landing on this property.</summary>
        public int BaseRent;
        
        // TODO: Add RentTiers[] for houses/hotels
        // TODO: Add ColorGroup (enum) for monopoly logic
    }
}