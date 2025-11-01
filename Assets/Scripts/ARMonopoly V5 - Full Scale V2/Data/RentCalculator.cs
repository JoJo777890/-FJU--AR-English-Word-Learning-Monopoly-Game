using UnityEngine;

namespace ARMonopoly_V5___Full_Scale_V2.Data
{
    [CreateAssetMenu(menuName = "ARMonopoly_V5___Full_Scale_V2/Rent Calculator")]
    public class RentCalculator : ScriptableObject
    {
        /// <summary>
        /// Calculates rent for a given property.
        /// </summary>
        public int CalculateRent(PropertyDef property)
        {
            // This is a simple implementation.
            // You can expand this to check for monopolies (full sets) or houses.
            if (property == null) return 0;

            return property.BaseRent;
        }
    }
}