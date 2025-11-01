using System.Collections.Generic;
using UnityEngine;

namespace ARMonopoly_V5___Full_Scale_V2.Data
{
    [CreateAssetMenu(menuName = "AR Monopoly/Spelling Question Database")]
    public class SpellingQuestionDatabase : ScriptableObject
    {
        public List<SpellingQuestion> Questions = new List<SpellingQuestion>();

        /// <summary>
        /// Gets a random question from the database.
        /// </summary>
        public SpellingQuestion GetRandomQuestion()
        {
            if (Questions.Count == 0)
            {
                Debug.LogError("SpellingQuestionDatabase is empty!");
                return null;
            }
            return Questions[Random.Range(0, Questions.Count)];
        }
    }
}