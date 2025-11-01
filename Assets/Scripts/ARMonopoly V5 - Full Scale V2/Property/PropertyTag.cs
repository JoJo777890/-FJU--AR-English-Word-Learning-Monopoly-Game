using ARMonopoly_V5___Full_Scale_V2.Data;
using UnityEngine;

namespace ARMonopoly_V5___Full_Scale_V2.Property
{
    /// <summary>
    /// Attached to a Property's Image Target.
    /// Links the physical target to its ScriptableObject data.
    /// </summary>
    public class PropertyTag : MonoBehaviour
    {
        public PropertyDef PropertyDefinition;

        // Helper property to ensure scripts always get the ID
        public string PropertyID => PropertyDefinition != null ? PropertyDefinition.PropertyID : "";
    }
}