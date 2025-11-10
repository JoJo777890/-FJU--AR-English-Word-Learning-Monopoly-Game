using UnityEngine;

namespace ARMonopoly_V5___Full_Scale_V2.Data
{
    /// <summary>
    /// ScriptableObject for rent calculation logic. Held by AppGame.
    /// Create one from 'Assets > Create > ARMonopoly_V5___Full_Scale_V2 > Rent Calculator'.
    /// </summary>
    [CreateAssetMenu(menuName = "ARMonopoly_V5___Full_Scale_V2/Rent Calculator")]
    public class RentCalculator : ScriptableObject
    {
        /// <summary>
        /// Calculates the rent for a given property.
        /// </summary>
        public int CalculateRent(PropertyDef property)
        {
            if (property == null) return 0;
            
            // TODO: Expand logic to check for monopolies (using Bank.GetPropertyOwner)
            // and houses/hotels once those are implemented.
            return property.BaseRent;
        }
    }
}