using UnityEngine;

namespace ARMonopoly_V5___Full_Scale_V2.Data
{
    /// <summary>
    /// Calculates rent. Held by AppGame.
    /// Create one from 'Assets > Create > AR Monopoly > Rent Calculator'.
    /// </summary>
    [CreateAssetMenu(menuName = "ARMonopoly_V5___Full_Scale_V2/Rent Calculator")]
    public class RentCalculator : ScriptableObject
    {
        public int CalculateRent(PropertyDef property)
        {
            if (property == null) return 0;
            
            // This is a simple implementation.
            // Future logic would check for monopolies, houses, etc.
            return property.BaseRent;
        }
    }
}