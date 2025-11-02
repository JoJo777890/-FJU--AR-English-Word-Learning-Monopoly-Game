using System.Collections.Generic;
using ARMonopoly_V5___Full_Scale_V2.Board;
using ARMonopoly_V5___Full_Scale_V2.Core;
using ARMonopoly_V5___Full_Scale_V2.Data;
using ARMonopoly_V5___Full_Scale_V2.Player;
using UnityEngine;

namespace ARMonopoly_V5___Full_Scale_V2.Gameplay
{
    public class TurnController : MonoBehaviour
    {
        [Header("Config")]
        public List<int> PlayerOrder = new List<int>();

        [Header("Runtime")]
        public int CurrentPlayerID { get; private set; }

        private int _turnIndex = -1;
        private GameStateMachine _stateMachine;
        private Dictionary<int, PlayerTag> _playerTags = new Dictionary<int, PlayerTag>();
        private BoardDefinition _board;

        private void Start()
        {
            _stateMachine = AppGame.Instance.StateMachine;
            _board = AppGame.Instance.Board;

            if (_board == null)
                Debug.LogError("TurnController: BoardDefinition is not assigned in AppGame!");
            if (_stateMachine == null)
                Debug.LogError("TurnController: StateMachine is null!");
            
            // Cache player tags for easy access
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

        /// <summary>
        /// Called by the RollButton. This just starts the move.
        /// </summary>
        public void OnRollClicked()
        {
            if (_stateMachine.CurrentState != GameState.PlayerTurn) return;

            // 1. Roll Dice
            int roll = Random.Range(1, 7) + Random.Range(1, 7);
            GameEvents.RaiseDiceRolled(CurrentPlayerID, roll);
            Debug.Log($"Player {CurrentPlayerID} rolled a {roll}");

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

            // 3. Calculate new position
            int newIndex = (oldIndex + roll) % boardSize;
            player.CurrentBoardIndex = newIndex; // Update the player's logical position

            // 4. Check for "Pass Go"
            if (newIndex < oldIndex) // They wrapped around
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
            AppGame.Instance.ExpectedDestinationPropertyID = destination.PropertyID;
            GameEvents.RaiseMoveRequired(new MovePayload
            {
                PlayerID = CurrentPlayerID,
                DestinationName = destination.DisplayName,
                DestinationPropertyID = destination.PropertyID
            });

            // 7. Change state to wait for the physical move
            _stateMachine.SetState(GameState.AwaitingPlayerMove);
        }

        /// <summary>
        /// This is now called by RuleEngine AFTER a landing is resolved or passed.
        /// </summary>
        public void EndTurn()
        {
            if (PlayerOrder.Count == 0)
            {
                Debug.LogError("TurnController 'PlayerOrder' list is empty!");
                return;
            }

            _turnIndex = (_turnIndex + 1) % PlayerOrder.Count;
            CurrentPlayerID = PlayerOrder[_turnIndex];

            _stateMachine.SetState(GameState.PlayerTurn);
            GameEvents.RaiseTurnStarted(CurrentPlayerID);
        }
    }
}
