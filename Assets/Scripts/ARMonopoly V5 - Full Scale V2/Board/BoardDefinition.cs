using System.Collections.Generic;
using ARMonopoly_V5___Full_Scale_V2.Data;
using UnityEngine;

namespace ARMonopoly_V5___Full_Scale_V2.Board
{
    /// <summary>
    /// Defines the logical order of properties around the board.
    /// Create one asset from 'Assets > Create > AR Monopoly > Board Definition'.
    /// </summary>
    [CreateAssetMenu(menuName = "ARMonopoly_V5___Full_Scale_V2/Board Definition")]
    public class BoardDefinition : ScriptableObject
    {
        [Tooltip("A list of all properties, in order, starting from 'Go'.")]
        public List<PropertyDef> PropertiesInOrder;

        public int TotalSpaces => PropertiesInOrder.Count;

        /// <summary>
        /// Gets the PropertyDef at a specific board index.
        /// </summary>
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
        /// Finds the board index for a given property ID.
        /// </summary>
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