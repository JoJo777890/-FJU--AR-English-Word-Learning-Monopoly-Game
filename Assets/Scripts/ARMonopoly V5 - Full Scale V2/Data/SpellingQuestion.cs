// In folder: ARMonopoly V5 - Full Scale V2/Data/

using UnityEngine;

namespace ARMonopoly_V5___Full_Scale_V2.Data
{
    [CreateAssetMenu(menuName = "ARMonopoly_V5___Full_Scale_V2/Spelling Question")]
    public class SpellingQuestion : ScriptableObject
    {
        public string QuestionText; // e.g., "Spell the word for a red fruit."
        public string CorrectAnswer; // e.g., "APPLE"
        public int FineAmount = 50;
    }
}

