using System.Collections.Generic;
using UnityEngine;
using Vuforia;

namespace ARMonopoly_V5___Full_Scale_V2.Spelling
{
    /// <summary>
    /// Scans for LetterTag objects in the scene and reconstructs words
    /// based on their proximity and alignment.
    /// </summary>
    public class ARCrosswordScanner : MonoBehaviour
    {
        // (Tip): Positional threshold to group letters into the same line (in meters).
        public float groupThreshold = 0.02f;

        /// <summary>
        /// Internal representation of a tracked letter.
        /// </summary>
        private class TrackedLetter
        {
            public char letter;
            public Vector3 position;
        }

        /// <summary>
        /// Main public method called by UIManager to get the scanned answer.
        /// </summary>
        /// <returns>The first detected horizontal word, or an empty string.</returns>
        public string GetFirstHorizontalWord()
        {
            List<TrackedLetter> letters = GetTrackedLetters();
            if (letters.Count == 0)
            {
                Debug.LogWarning("CrosswordScanner: No 'SpellingLetter' tags found.");
                return "";
            }

            string allRows = ScanRows(letters);
            if (string.IsNullOrEmpty(allRows))
            {
                return "";
            }
            
            // Return only the first line from the horizontal scan
            string[] rows = allRows.Split('\n');
            return rows.Length > 0 ? rows[0] : "";
        }

        /// <summary>
        /// Finds all active and tracked GameObjects tagged as "SpellingLetter".
        /// </summary>
        private List<TrackedLetter> GetTrackedLetters()
        {
            List<TrackedLetter> trackedLetters = new List<TrackedLetter>();
            ObserverBehaviour[] allTargets = FindObjectsOfType<ObserverBehaviour>();

            foreach (var tb in allTargets)
            {
                // 1. Only look at tracked objects
                if (tb.TargetStatus.Status == Status.TRACKED) 
                {
                    // 2. Only look at objects with the "SpellingLetter" tag
                    if (tb.gameObject.CompareTag("SpellingLetter"))
                    {
                        // 3. Get the letter from the LetterTag component
                        LetterTag tag = tb.gameObject.GetComponent<LetterTag>();
                        if (tag != null)
                        {
                            trackedLetters.Add(new TrackedLetter
                            {
                                letter = char.ToUpper(tag.Letter),
                                position = tb.gameObject.transform.position
                            });
                        }
                    }
                }
            }
            return trackedLetters;
        }

        /// <summary>
        /// Groups letters into horizontal rows and sorts them left-to-right.
        /// </summary>
        private string ScanRows(List<TrackedLetter> letters)
        {
            List<List<TrackedLetter>> rows = new List<List<TrackedLetter>>();

            // Group letters into rows based on Y-position
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
                {
                    rows.Add(new List<TrackedLetter> { letter });
                }
            }

            // Sort rows top-to-bottom
            rows.Sort((a, b) => b[0].position.y.CompareTo(a[0].position.y));
            // Sort letters in each row left-to-right
            foreach (var row in rows)
            {
                row.Sort((a, b) => a.position.x.CompareTo(b.position.x));
            }

            // Concatenate into a string
            string result = "";
            foreach (var row in rows)
            {
                foreach (var l in row)
                    result += l.letter;
                result += "\n";
            }

            return result.Trim();
        }

        /// <summary>
        /// Groups letters into vertical columns and sorts them top-to-bottom.
        /// (Currently unused but available for future crossword logic).
        /// </summary>
        private string ScanColumns(List<TrackedLetter> letters)
        {
            List<List<TrackedLetter>> columns = new List<List<TrackedLetter>>();

            // Group letters into columns based on X-position
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
                    columns.Add(new List<TrackedLetter> { letter });
                }
            }
            
            // Sort columns left-to-right
            columns.Sort((a, b) => a[0].position.x.CompareTo(b[0].position.x));
            // Sort letters in each column top-to-bottom
            foreach (var col in columns)
            {
                col.Sort((a, b) => b.position.y.CompareTo(a.position.y));
            }

            // Concatenate into a string
            string result = "";
            foreach (var col in columns)
            {
                foreach (var l in col)
                    result += l.letter;
                result += "\n";
            }

            return result.Trim();
        }
    }
}