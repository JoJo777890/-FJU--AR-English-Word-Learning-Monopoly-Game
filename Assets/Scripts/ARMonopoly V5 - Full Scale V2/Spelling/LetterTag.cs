using UnityEngine;

namespace ARMonopoly_V5___Full_Scale_V2.Spelling
{
    /// <summary>
    /// Attach this component to your letter Image Targets (A, B, C...).
    /// This identifies them as spelling letters for the ARCrosswordScanner.
    /// Don't forget to also assign the "SpellingLetter" Unity Tag.
    /// </summary>
    public class LetterTag : MonoBehaviour
    {
        [Tooltip("The letter this target represents.")]
        public char Letter;
    }
}