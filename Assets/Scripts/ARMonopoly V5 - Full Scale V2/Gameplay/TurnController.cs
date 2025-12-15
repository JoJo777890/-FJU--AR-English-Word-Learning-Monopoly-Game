using System.Collections.Generic;
using ARMonopoly_V5___Full_Scale_V2.Board;
using ARMonopoly_V5___Full_Scale_V2.Core;
using ARMonopoly_V5___Full_Scale_V2.Data;
using ARMonopoly_V5___Full_Scale_V2.Player;
using UnityEngine;

namespace ARMonopoly_V5___Full_Scale_V2.Gameplay
{
    /// <summary>
    /// Manages the player turn order and dice rolling logic.
    /// Attached to the [GameSystems] GameObject.
    /// Dependencies are injected by AppGame.
    /// </summary>
    public class TurnController : MonoBehaviour
    {
        [Header("Config")]
        // (Tip): The order of PlayerIDs for turns (e.g., 1, 2)
        public List<int> PlayerOrder = new List<int>();

        [Header("Runtime")]
        /// <summary>The PlayerID of the player whose turn it is.</summary>
        public int CurrentPlayerID { get; private set; }

        private int _turnIndex = -1;
        private GameStateMachine _stateMachine;
        private Dictionary<int, PlayerTag> _playerTags = new Dictionary<int, PlayerTag>();
        private BoardDefinition _board;
        private DiceScanner _diceScanner;
        private AppGame _appGameContext; // Only for ExpectedDestinationPropertyID

        /// <summary>
        /// Injection Method. Called by AppGame.
        /// </summary>
        public void Construct(GameStateMachine stateMachine, BoardDefinition board, DiceScanner scanner, AppGame appGame)
        {
            _stateMachine = stateMachine;
            _board = board;
            _diceScanner = scanner;
            _appGameContext = appGame;

            // Cache all PlayerTag components in the scene for fast lookup
            foreach (var playerTag in FindObjectsOfType<PlayerTag>())
            {
                if (playerTag != null)
                {
                    Debug.Log($"TurnController: Caching Player {playerTag.PlayerID}");
                    _playerTags[playerTag.PlayerID] = playerTag;
                }
            }

            // Start the first turn
            EndTurn();
        }

        private void OnEnable() 
        {
            GameEvents.OnDiceRolled += HandleDiceRoll;
        }
        private void OnDisable()
        {
            GameEvents.OnDiceRolled -= HandleDiceRoll;
        }

        /// <summary>
        /// Public method called by the RollButton UI.
        /// This now *starts* the scan, it doesn't calculate the roll.
        /// </summary>
        public void OnRollClicked()
        {
            if (_stateMachine.CurrentState != GameState.PlayerTurn) return;
            
            // 1. Change state to AwaitingDiceRoll
            _stateMachine.SetState(GameState.AwaitingDiceRoll);
            
            // 2. Tell the scanner to start looking
            _diceScanner.StartScan(CurrentPlayerID);
        }

        /// <summary>
        /// This method now runs *after* the DiceScanner fires OnDiceRolled.
        /// </summary>
        private void HandleDiceRoll(int playerID, int totalRoll)
        {
            // Only react if it's the current player's roll
            if (playerID != CurrentPlayerID) return;
            
            // 1. Log the physical roll
            GameEvents.RaiseLogMessage($"Player {CurrentPlayerID} rolled a {totalRoll}.");

            // 2. Get Player's current position
            if (!_playerTags.ContainsKey(CurrentPlayerID))
            {
                Debug.LogError($"PlayerTag for PlayerID {CurrentPlayerID} not found!");
                return;
            }

            PlayerTag player = _playerTags[CurrentPlayerID];
            int oldIndex = player.CurrentBoardIndex;
            int boardSize = _board.TotalSpaces;
            
            if (boardSize == 0)
            {
                Debug.LogError("BoardDefinition has 0 properties in its list!");
                return;
            }

            // 3. Calculate new logical position (using the physical 'totalRoll')
            int newIndex = (oldIndex + totalRoll) % boardSize;
            player.CurrentBoardIndex = newIndex;

            // 4. Check for "Pass Go"
            if (newIndex < oldIndex) // They wrapped around the board
            {
                GameEvents.RaisePlayerPassedGo(CurrentPlayerID);
            }

            // 5. Get the destination property
            PropertyDef destination = _board.GetPropertyAt(newIndex);
            if (destination == null)
            {
                Debug.LogError($"No property found at index {newIndex}!");
                return;
            }
            
            // 6. Store expected destination and notify UI
            _appGameContext.ExpectedDestinationPropertyID = destination.PropertyID;
            GameEvents.RaiseMoveRequired(new MovePayload
            {
                PlayerID = CurrentPlayerID,
                DestinationName = destination.DisplayName,
                DestinationPropertyID = destination.PropertyID
            });
            
            GameEvents.RaiseLogMessage($"Player {CurrentPlayerID} must move to {destination.DisplayName}.");
            
            // 7. Change state to wait for the physical move
            _stateMachine.SetState(GameState.AwaitingPlayerMove);
        }

        /// <summary>
        /// Called by RuleEngine AFTER a landing is resolved or passed.
        /// </summary>
        public void EndTurn()
        {
            if (PlayerOrder.Count == 0)
            {
                Debug.LogError("TurnController 'PlayerOrder' list is empty!");
                return;
            }

            // Advance to the next player
            _turnIndex = (_turnIndex + 1) % PlayerOrder.Count;
            CurrentPlayerID = PlayerOrder[_turnIndex];

            // Set state and fire events for the new turn
            _stateMachine.SetState(GameState.PlayerTurn);
            GameEvents.RaiseLogMessage($"--- Player {CurrentPlayerID}'s Turn Begins ---");
            GameEvents.RaiseTurnStarted(CurrentPlayerID);
        }
    }
}