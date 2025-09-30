// Assets/Scripts/Simple/PropertyTag.cs
using UnityEngine;

namespace ARMonopoly.Simple
{
    public class PropertyTag : MonoBehaviour
    {
        [Header("Property")]
        public string propertyId = "PROP_001";
        public string displayName = "Place A";
        public int price = 100;
        public int baseRent = 20;

        [Header("Visuals")]
        [Tooltip("Scale only children whose names end with this suffix (e.g., Plane).")]
        public string contentSuffix = "Plane";
    }
}