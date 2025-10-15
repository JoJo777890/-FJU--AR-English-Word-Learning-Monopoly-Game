using UnityEngine;

namespace ARMonopoly_V4___Full_Scale_V1.Data
{
    [CreateAssetMenu(menuName="ARMonopolyV4-FullScale/Property")]
    public class PropertyDef : ScriptableObject
    {
        public string id;               // stable key (e.g., "TOKYO_TOWER")
        public string displayName;
        public int price = 100;
        public int[] rentTiers = {20};  // index 0 = base (no houses)
        public bool isUtility = false;
        public bool isRailroad = false;
        public string colorSet = "None";
    }
}