using ARMonopoly_V5___Full_Scale_V2.Data;
using UnityEngine;

namespace ARMonopoly_V5___Full_Scale_V2.Property
{
    /// <summary>
    /// Links a property's Image Target in the scene to its ScriptableObject data.
    /// Attached to the Property's Image Target.
    /// </summary>
    [RequireComponent(typeof(PropertyVisuals))]
    public class PropertyTag : MonoBehaviour
    {
        // (Tip): The ScriptableObject asset that defines this property (e.g., 'Boardwalk_Def').
        public PropertyDef Definition;

        /// <summary>
        /// Public getter for other scripts to read this property's unique ID.
        /// </summary>
        public string PropertyID
        {
            get
            {
                if (Definition != null)
                    return Definition.PropertyID;
                
                // Fallback in case the Definition is not assigned in the Inspector
                Debug.LogWarning($"PropertyTag on {gameObject.name} is missing its 'Definition' asset!", this);
                return gameObject.name; 
            }
        }

        /// <summary>
        /// Public getter for other scripts to read this property's display name.
        /// </summary>
        public string DisplayName
        {
            get
            {
                if (Definition != null)
                    return Definition.DisplayName;
                
                return gameObject.name; // Fallback
            }
        }
    }
}