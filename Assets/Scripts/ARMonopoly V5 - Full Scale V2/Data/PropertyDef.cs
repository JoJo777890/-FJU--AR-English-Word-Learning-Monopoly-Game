using UnityEngine;

namespace ARMonopoly_V5___Full_Scale_V2.Data
{
    [CreateAssetMenu(menuName = "ARMonopoly_V5___Full_Scale_V2/Property Definition")]
    public class PropertyDef : ScriptableObject
    {
        [Header("Board Data")]
        public string PropertyID; // "PROP_A", "PROP_B", etc.
        public string DisplayName;

        [Header("Economic Data")]
        public int Price;
        public int BaseRent;
        // You can add rent tiers, house costs, etc. here
    }
}