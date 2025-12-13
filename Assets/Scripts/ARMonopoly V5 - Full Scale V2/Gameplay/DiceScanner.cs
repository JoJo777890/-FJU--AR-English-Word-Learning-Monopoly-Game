using UnityEngine;
using Vuforia;
using System.Collections.Generic;
using ARMonopoly_V5___Full_Scale_V2.Core;
using ARMonopoly_V5___Full_Scale_V2.Data;

namespace ARMonopoly_V5___Full_Scale_V2.Gameplay
{
    /// <summary>
    /// Scans for physical dice faces (Image Targets) to determine a roll.
    /// Determines the result by checking which face's normal vector is aligned with World Up.
    /// </summary>
    public class DiceScanner : MonoBehaviour
    {
        [Header("Data")]
        [Tooltip("Assign all 6 of your DiceFace ScriptableObjects here.")]
        public List<DiceFace> DiceFaces;

        [Header("Scanning Config")]
        [Tooltip("How long the die face must be seen to confirm the roll.")]
        public float DwellTime = 0.5f;
        
        [Tooltip("Minimum alignment required (0-1) to consider a face 'up'. 0.8 is roughly 35 degrees.")]
        public float MinAlignment = 0.7f;

        private Dictionary<string, int> _faceValueMap = new Dictionary<string, int>();
        private ObserverBehaviour[] _allObservers;
        private bool _isScanning = false;
        private bool _waitingForClearance = false;
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
            
            // --- CLEARANCE CHECK ---
            // Check if we can see a die RIGHT NOW. 
            // If yes, we must wait for the player to pick it up (break tracking) 
            // before we accept a new roll.
            if (IsAnyFaceTracked())
            {
                _waitingForClearance = true;
                GameEvents.RaiseLogMessage("Please pick up the dice to roll!");
            }
            else
            {
                _waitingForClearance = false;
                GameEvents.RaiseLogMessage("Roll the dice now!");
            }
        }

        private void StopScan()
        {
            _isScanning = false;
            _waitingForClearance = false;
        }

        void Update()
        {
            if (!_isScanning) return;

            // --- 1. HANDLE CLEARANCE ---
            if (_waitingForClearance)
            {
                // We are waiting for the player to pick up the old dice.
                // We stay in this state until NO dice are tracked.
                if (!IsAnyFaceTracked())
                {
                    _waitingForClearance = false;
                    GameEvents.RaiseLogMessage("Dice cleared. Rolling...");
                }
                return; // Don't process any rolls yet
            }

            // --- 2. NORMAL SCANNING ---
            float bestAlignment = -1.0f;
            ObserverBehaviour faceUpTarget = null;

            // Iterate through all Vuforia targets to find the one facing "Up"
            foreach (var observer in _allObservers)
            {
                // 1. Is this a die face we know about?
                // 2. Is Vuforia currently tracking it?
                if (_faceValueMap.ContainsKey(observer.gameObject.name) && 
                    observer.TargetStatus.Status == Status.TRACKED)
                {
                    // Calculate the Dot Product between the target's Normal (transform.up) and World Up.
                    // Result: 1.0 = Facing Up, 0.0 = Sideways, -1.0 = Facing Down.
                    float alignment = Vector3.Dot(observer.transform.up, Vector3.up);

                    // We look for the face with the highest alignment value
                    if (alignment > bestAlignment)
                    {
                        bestAlignment = alignment;
                        faceUpTarget = observer;
                    }
                }
            }

            // Only proceed if we found a valid candidate that is mostly facing up
            if (faceUpTarget != null && bestAlignment > MinAlignment)
            {
                int currentRoll = _faceValueMap[faceUpTarget.gameObject.name];
                
                // Track how long this specific number has been the "winner"
                if (!_rollVotes.ContainsKey(currentRoll))
                {
                    _rollVotes[currentRoll] = 0;
                }

                _rollVotes[currentRoll] += Time.deltaTime;

                // If this face has been up for the DwellTime (e.g. 0.5s), confirm the roll
                if (_rollVotes[currentRoll] >= DwellTime)
                {
                    Debug.Log($"[DiceScanner] Confirmed Roll: {currentRoll} (Alignment: {bestAlignment:F2})");
                    StopScan();
                    // Fire the event to resume the game
                    GameEvents.RaiseDiceRolled(_currentPlayerID, currentRoll);
                }
            }
            else
            {
                // If the die is tumbling or held at a weird angle, reset votes
                // This ensures we only accept a roll when the die settles flat.
                _rollVotes.Clear();
            }
        }
        
        /// <summary>
        /// Helper to check if ANY of our die faces are currently visible.
        /// </summary>
        private bool IsAnyFaceTracked()
        {
            foreach (var observer in _allObservers)
            {
                if (_faceValueMap.ContainsKey(observer.gameObject.name) && 
                    observer.TargetStatus.Status == Status.TRACKED)
                {
                    return true;
                }
            }
            return false;
        }
    }
}