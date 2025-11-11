using UnityEngine;

namespace ARMonopoly_V5___Full_Scale_V2.Data
{
    /// <summary>
    /// ScriptableObject defining a single spelling question.
    /// Create one from 'Assets > Create > ARMonopoly_V5___Full_Scale_V2 > Spelling Question'.
    /// </summary>
    [CreateAssetMenu(menuName = "ARMonopoly_V5___Full_Scale_V2/Spelling Question")]
    public class SpellingQuestion : ScriptableObject
    {
        // (Tip): The question text shown to the player (e.g., 'Spell the word for a red fruit.')
        public string QuestionText;
        // (Tip): The correct answer (not case-sensitive).
        public string CorrectAnswer;
        // (Tip): The fine amount if answered incorrectly on an unowned property.
        public int FineAmount = 50;
    }
}