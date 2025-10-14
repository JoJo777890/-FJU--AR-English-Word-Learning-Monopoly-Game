// Assets/Scripts/Simple/PropertyTag.cs
using UnityEngine;

namespace ARMonopoly.Simple
{
    public class PropertyTag : MonoBehaviour
    {
        [Header("Property")]
        public string propertyId = "PROP_A";
        public string displayName = "Place A";
        public int price = 100;
        public int baseRent = 20;

        [Header("Visuals")]
        public string contentSuffix = "Plane";
    }
}