using UnityEngine;

namespace ARMonopoly_Medium_Scale
{
    [CreateAssetMenu(menuName="ARMonopoly/Property")]
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