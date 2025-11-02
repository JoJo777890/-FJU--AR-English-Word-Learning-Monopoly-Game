using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using Vuforia;

namespace ARMonopoly_V5___Full_Scale_V2.Spelling
{
    /// <summary>
    /// Modified version of your script to act as an on-demand service.
    /// The RuleEngine will call GetScannedWords() when it needs to check an answer.
    /// </summary>
    public class ARCrosswordScanner : MonoBehaviour
    {
        [Header("Scanning Config")]
        public TMP_Text debugText;
        public float groupThreshold = 0.02f;
        public string letterTargetPrefix = "ImageTarget";

        private List<TrackedLetter> _trackedLetters = new List<TrackedLetter>();

        // Internal class for sorting
        private class TrackedLetter
        {
            public char letter;
            public Vector3 position;
        }

        /// <summary>
        /// Main public method for the RuleEngine to call.
        /// Returns all scannable words, with horizontal words first.
        /// </summary>
        public List<string> GetScannedWords()
        {
            FindAllTrackedLetters();

            if (_trackedLetters.Count == 0)
            {
                if (debugText != null) debugText.text = "No letters found.";
                return new List<string>();
            }

            string horizontal = ScanRows(_trackedLetters);
            string vertical = ScanColumns(_trackedLetters);

            string output = $"Horizontal:\n{horizontal}\nVertical:\n{vertical}";
            Debug.Log(output);
            if (debugText != null) debugText.text = output;

            // Combine all found words into a list for checking
            List<string> foundWords = new List<string>();
            
            // Add non-empty horizontal words
            if (!string.IsNullOrEmpty(horizontal))
                foundWords.AddRange(horizontal.Split('\n'));
            
            // Add non-empty vertical words
            if (!string.IsNullOrEmpty(vertical))
                foundWords.AddRange(vertical.Split('\n'));

            return foundWords;
        }

        private void FindAllTrackedLetters()
        {
            _trackedLetters.Clear();
            
            ObserverBehaviour[] allTargets = FindObjectsOfType<ObserverBehaviour>();

            foreach (var obs in allTargets)
            {
                if (obs.TargetStatus.Status == Status.TRACKED && 
                    obs.gameObject.name.ToLower().StartsWith(letterTargetPrefix.ToLower()))
                {
                    string objName = obs.gameObject.name;
                    char letter = objName[objName.Length - 1];
                    letter = char.ToUpper(letter);

                    _trackedLetters.Add(new TrackedLetter
                    {
                        letter = letter,
                        position = obs.gameObject.transform.position
                    });
                }
            }
        }

        private string ScanRows(List<TrackedLetter> letters)
        {
            List<List<TrackedLetter>> rows = new List<List<TrackedLetter>>();

            foreach (var letter in letters)
            {
                bool added = false;
                foreach (var row in rows)
                {
                    if (Mathf.Abs(letter.position.y - row[0].position.y) < groupThreshold)
                    {
                        row.Add(letter);
                        added = true;
                        break;
                    }
                }
                if (!added)
                    rows.Add(new List<TrackedLetter> { letter });
            }

            rows.Sort((a, b) => b[0].position.y.CompareTo(a[0].position.y));
            foreach (var row in rows)
                row.Sort((a, b) => a.position.x.CompareTo(b.position.x));

            StringBuilder result = new StringBuilder();
            foreach (var row in rows)
            {
                foreach (var l in row)
                    result.Append(l.letter);
                result.Append('\n');
            }

            return result.ToString().Trim();
        }

        private string ScanColumns(List<TrackedLetter> letters)
        {
            List<List<TrackedLetter>> columns = new List<List<TrackedLetter>>();

            foreach (var letter in letters)
            {
                bool added = false;
                foreach (var col in columns)
                {
                    if (Mathf.Abs(letter.position.x - col[0].position.x) < groupThreshold)
                    {
                        col.Add(letter);
                        added = true;
                        break;
                    }
                }
                if (!added)
                {
                    // **BUG FIX 1: Was adding List<List<...>>**
                    columns.Add(new List<TrackedLetter> { letter });
                }
            }

            columns.Sort((a, b) => a[0].position.x.CompareTo(b[0].position.x));
            foreach (var col in columns)
            {
                // **BUG FIX 2: Was `a[0].position.y`**
                col.Sort((a, b) => b.position.y.CompareTo(a.position.y));
            }

            StringBuilder result = new StringBuilder();
            foreach (var col in columns)
            {
                foreach (var l in col)
                    result.Append(l.letter);
                result.Append('\n');
            }

            return result.ToString().Trim();
        }
    }
}

