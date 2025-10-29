using ARMonopoly_V5___Full_Scale_V2.Data;
using UnityEngine;

namespace ARMonopoly_V5___Full_Scale_V2.Property
{
    /// <summary>
    /// Attach to each Property ImageTarget.
    /// Links the physical target to its data definition.
    /// </summary>
    public class PropertyTag : MonoBehaviour
    {
        public PropertyDef PropertyDefinition;

        public string ID => PropertyDefinition != null ? PropertyDefinition.PropertyID : "INVALID";
    }
}