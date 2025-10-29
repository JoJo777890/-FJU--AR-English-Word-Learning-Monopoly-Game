using UnityEngine;

namespace ARMonopoly_V5___Full_Scale_V2.Data
{
    /// <summary>
    /// Encapsulates rent rules. For now, it's very simple.
    /// By making it an SO, you can swap it for a more complex one later
    /// (e.g., one that checks for monopolies) without changing other code.
    /// </summary>
    [CreateAssetMenu(menuName = "AR Monopoly/Rent Calculator")]
    public class RentCalculator : ScriptableObject
    {
        public int CalculateRent(PropertyDef property)
        {
            if (property == null) return 0;
            
            // This is where you would add complex logic, e.g.:
            // int ownerId = OwnershipService.GetOwner(property.PropertyID);
            // bool hasMonopoly = OwnershipService.HasMonopoly(ownerId, property.ColorSet);
            // int houses = OwnershipService.GetHouseCount(property.PropertyID);
            // int rent = property.RentTiers[houses];
            // if (hasMonopoly && houses == 0) rent *= 2;
            
            // For this version, we just return base rent.
            return property.BaseRent;
        }
    }
}