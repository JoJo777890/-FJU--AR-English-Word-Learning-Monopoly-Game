using UnityEngine;

namespace ARMonopoly_V5___Full_Scale_V2.Data
{
    [CreateAssetMenu(menuName = "ARMonopoly_V5___Full_Scale_V2/Spelling Question")]
    public class SpellingQuestion : ScriptableObject
    {
        [Tooltip("The question/clue to show the player.")]
        [TextArea(3, 5)]
        public string QuestionText;

        [Tooltip("The exact, all-caps word the player must spell.")]
        public string CorrectAnswer;
    }
}