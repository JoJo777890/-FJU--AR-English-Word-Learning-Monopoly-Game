// In folder: ARMonopoly V5 - Full Scale V2/Data/

using System.Collections.Generic;
using UnityEngine;

namespace ARMonopoly_V5___Full_Scale_V2.Data
{
    [CreateAssetMenu(menuName = "ARMonopoly_V5___Full_Scale_V2/Spelling Question Database")]
    public class SpellingQuestionDatabase : ScriptableObject
    {
        public List<SpellingQuestion> AllQuestions;
    
        public SpellingQuestion GetRandomQuestion()
        {
            if (AllQuestions == null || AllQuestions.Count == 0) return null;
            return AllQuestions[Random.Range(0, AllQuestions.Count)];
        }
    }
}

