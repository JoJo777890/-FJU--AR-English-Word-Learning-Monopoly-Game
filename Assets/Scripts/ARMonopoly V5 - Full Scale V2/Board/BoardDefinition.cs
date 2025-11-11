using System.Collections.Generic;
using ARMonopoly_V5___Full_Scale_V2.Data;
using UnityEngine;

namespace ARMonopoly_V5___Full_Scale_V2.Board
{
    /// <summary>
    /// ScriptableObject that defines the logical sequence of properties on the board.
    /// This is crucial for calculating movement.
    /// Create one from 'Assets > Create > ARMonopoly_V5___Full_Scale_V2 > Board Definition'.
    /// </summary>
    [CreateAssetMenu(menuName = "ARMonopoly_V5___Full_Scale_V2/Board Definition")]
    public class BoardDefinition : ScriptableObject
    {
        // The complete list of PropertyDef assets, in clockwise order, starting from 'Go'
        public List<PropertyDef> PropertiesInOrder;

        /// <summary>
        /// Gets the total number of spaces on the board.
        /// </summary>
        public int TotalSpaces => PropertiesInOrder.Count;

        /// <summary>
        /// Gets the PropertyDef at a specific logical board index.
        /// </summary>
        /// <param name="index">The board index (0 = Go).</param>
        public PropertyDef GetPropertyAt(int index)
        {
            if (PropertiesInOrder == null || TotalSpaces == 0) return null;

            if (index >= 0 && index < TotalSpaces)
            {
                return PropertiesInOrder[index];
            }
            Debug.LogError($"BoardDefinition: Index {index} is out of range.");
            return null;
        }

        /// <summary>
        /// Finds the board index for a given property's unique string ID.
        /// </summary>
        /// <returns>The index (0 to TotalSpaces-1), or -1 if not found.</returns>
        public int GetIndexFromID(string propertyID)
        {
            if (PropertiesInOrder == null) return -1;
            
            for (int i = 0; i < TotalSpaces; i++)
            {
                if (PropertiesInOrder[i] != null && PropertiesInOrder[i].PropertyID == propertyID)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}