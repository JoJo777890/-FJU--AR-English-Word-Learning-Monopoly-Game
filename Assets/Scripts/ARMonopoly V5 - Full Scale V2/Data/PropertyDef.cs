using UnityEngine;

namespace ARMonopoly_V5___Full_Scale_V2.Data
{
    public enum PropertyType { Normal, Utility, Railroad, Special }

    [CreateAssetMenu(menuName = "AR Monopoly/Property Definition")]
    public class PropertyDef : ScriptableObject
    {
        [Header("Identity")]
        public string PropertyID; // A stable, unique ID like "PROP_ boardwalk"
        public string DisplayName;
        public PropertyType Type = PropertyType.Normal;

        [Header("Economy")]
        public int Price = 0;
        public int BaseRent = 0;
        // You could expand this later with:
        // public int[] RentTiers;
        // public string ColorSet;
    }
}