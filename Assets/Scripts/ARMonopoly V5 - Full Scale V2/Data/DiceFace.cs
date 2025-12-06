using UnityEngine;

namespace ARMonopoly_V5___Full_Scale_V2.Data
{
    /// <summary>
    /// ScriptableObject to map a Vuforia Image Target's name to a dice value.
    /// Create one for each face (1-6).
    /// </summary>
    [CreateAssetMenu(menuName = "ARMonopoly_V5___Full_Scale_V2/Dice Face")]
    public class DiceFace : ScriptableObject
    {
        [Tooltip("The exact name of the Vuforia Image Target GameObject for this face.")]
        public string VuforiaTargetName;
        
        [Tooltip("The number this face represents (1-6).")]
        public int DiceValue;
    }
}