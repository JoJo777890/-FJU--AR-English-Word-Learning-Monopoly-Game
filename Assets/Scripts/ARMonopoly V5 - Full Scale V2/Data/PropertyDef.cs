using UnityEngine;

namespace ARMonopoly_V5___Full_Scale_V2.Data
{
    /// <summary>
    /// Defines a single property.
    /// Create one from 'Assets > Create > AR Monopoly > Property Definition'.
    /// </summary>
    [CreateAssetMenu(menuName = "ARMonopoly_V5___Full_Scale_V2/Property Definition")]
    public class PropertyDef : ScriptableObject
    {
        [Tooltip("Unique ID, e.g., 'BOARDWALK'. Must match ID in BoardDefinition.")]
        public string PropertyID;
        public string DisplayName;
        public int Price;
        public int BaseRent;
        // Future: public int[] RentTiers;
        // Future: public string ColorGroup;
    }
}