using UnityEngine;

namespace ARMonopoly.Data
{
    [CreateAssetMenu(fileName = "PropertyDef", menuName = "ARMonopoly/PropertyDef")]
    public class PropertyDef : ScriptableObject
    {
        public string id;
        public string displayName;
        public string colorsSet;
        public int purchasePrice;
        // public int mortgageValue;
        public int houseCost;
        public int hotelCost;
        public int[] baseRents; // [0]=no house, [1..4]=houses, [5]=hotel
        // public bool isUtility;
        // public bool isRailroad;
    }
}

