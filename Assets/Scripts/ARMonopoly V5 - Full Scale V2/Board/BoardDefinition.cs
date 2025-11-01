using System.Collections.Generic;
using ARMonopoly_V5___Full_Scale_V2.Data;
using UnityEngine;

// This namespace matches the user's provided TurnController
namespace ARMonopoly_V5___Full_Scale_V2.Board 
{
    [CreateAssetMenu(menuName = "ARMonopoly_V5___Full_Scale_V2/Board Definition")]
    public class BoardDefinition : ScriptableObject
    {
        [Tooltip("A list of all properties, in the exact order they appear on the board (e.g., Go, Prop1, Prop2...)")]
        public List<PropertyDef> PropertiesInOrder;

        public int TotalSpaces => PropertiesInOrder.Count;

        /// <summary>
        /// Gets the PropertyDef at a specific board index (0 to TotalSpaces-1).
        /// </summary>
        public PropertyDef GetPropertyAt(int index)
        {
            if (PropertiesInOrder == null || TotalSpaces == 0) return null;
            
            // Handle wrap-around just in case
            int wrappedIndex = index % TotalSpaces;
            if (wrappedIndex < 0) wrappedIndex += TotalSpaces;
            
            return PropertiesInOrder[wrappedIndex];
        }

        /// <summary>
        /// Finds the board index for a given Property ID.
        /// </summary>
        /// <returns>The index (0 to N-1), or -1 if not found.</returns>
        public int GetIndexFromID(string propertyID)
        {
            if (string.IsNullOrEmpty(propertyID) || PropertiesInOrder == null)
                return -1;

            for (int i = 0; i < PropertiesInOrder.Count; i++)
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