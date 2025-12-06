using UnityEngine;
using Vuforia;
using System.Collections.Generic;
using ARMonopoly_V5___Full_Scale_V2.Core;
using ARMonopoly_V5___Full_Scale_V2.Data;

namespace ARMonopoly_V5___Full_Scale_V2.Gameplay
{
    /// <summary>
    /// Scans for physical dice faces (Image Targets) to determine a roll.
    /// Assumes the "face up" target is the one with the highest Y-position.
    /// </summary>
    public class DiceScanner : MonoBehaviour
    {
        [Header("Data")]
        [Tooltip("Assign all 6 of your DiceFace ScriptableObjects here.")]
        public List<DiceFace> DiceFaces;

        [Header("Scanning Config")]
        [Tooltip("How long the die face must be seen to confirm the roll.")]
        public float DwellTime = 0.5f;

        private Dictionary<string, int> _faceValueMap = new Dictionary<string, int>();
        private ObserverBehaviour[] _allObservers;
        private bool _isScanning = false;
        private int _currentPlayerID;

        // Dwell time tracking
        private Dictionary<int, float> _rollVotes = new Dictionary<int, float>();

        void Start()
        {
            // Create a fast-lookup map from the target name to its dice value
            foreach (var face in DiceFaces)
            {
                if (face != null && !string.IsNullOrEmpty(face.VuforiaTargetName))
                {
                    _faceValueMap[face.VuforiaTargetName] = face.DiceValue;
                }
            }
            
            // Find all image targets in the scene
            _allObservers = FindObjectsOfType<ObserverBehaviour>();
        }

        /// <summary>
        /// Called by TurnController to start scanning for a roll.
        /// </summary>
        public void StartScan(int playerID)
        {
            _currentPlayerID = playerID;
            _isScanning = true;
            _rollVotes.Clear();
            GameEvents.RaiseLogMessage("Please roll your physical die.");
        }

        private void StopScan()
        {
            _isScanning = false;
        }

        void Update()
        {
            if (!_isScanning) return;

            float highestY = -float.MaxValue;
            ObserverBehaviour faceUpTarget = null;

            // Find the highest *tracked* dice face
            foreach (var observer in _allObservers)
            {
                if (observer.TargetStatus.Status == Status.TRACKED && 
                    _faceValueMap.ContainsKey(observer.gameObject.name))
                {
                    if (observer.transform.position.y > highestY)
                    {
                        highestY = observer.transform.position.y;
                        faceUpTarget = observer;
                    }
                }
            }

            if (faceUpTarget != null)
            {
                // We have a "face up" target, now let's see if it's stable
                int currentRoll = _faceValueMap[faceUpTarget.gameObject.name];
                
                if (!_rollVotes.ContainsKey(currentRoll))
                {
                    _rollVotes[currentRoll] = 0;
                }

                _rollVotes[currentRoll] += Time.deltaTime;

                // If it's been the highest face for our dwell time, confirm it!
                if (_rollVotes[currentRoll] >= DwellTime)
                {
                    StopScan();
                    // Fire the event that TurnController is waiting for
                    GameEvents.RaiseDiceRolled(_currentPlayerID, currentRoll);
                }
            }
        }
    }
}