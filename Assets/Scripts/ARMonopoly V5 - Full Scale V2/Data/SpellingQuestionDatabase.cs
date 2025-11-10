using System.Collections.Generic;
using UnityEngine;

namespace ARMonopoly_V5___Full_Scale_V2.Data
{
    /// <summary>
    /// ScriptableObject database holding all SpellingQuestion assets.
    /// Create one from 'Assets > Create > ARMonopoly_V5___Full_Scale_V2 > Spelling Question Database'.
    /// </summary>
    [CreateAssetMenu(menuName = "ARMonopoly_V5___Full_Scale_V2/Spelling Question Database")]
    public class SpellingQuestionDatabase : ScriptableObject
    {
        [Tooltip("The list of all possible spelling questions.")]
        public List<SpellingQuestion> AllQuestions;
    
        /// <summary>
        /// Gets a random question from the database.
        /// </summary>
        public SpellingQuestion GetRandomQuestion()
        {
            if (AllQuestions == null || AllQuestions.Count == 0) return null;
            return AllQuestions[Random.Range(0, AllQuestions.Count)];
        }
    }
}