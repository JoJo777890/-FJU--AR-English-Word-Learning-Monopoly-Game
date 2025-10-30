using System.Collections.Generic;
using ARMonopoly_V5___Full_Scale_V2.Core;
using ARMonopoly_V5___Full_Scale_V2.Data;
using UnityEngine;

namespace ARMonopoly_V5___Full_Scale_V2.Gameplay
{
    /// <summary>
    /// Manages the flow of turns, dice rolls, and jail status.
    /// </summary>
    public class TurnController : MonoBehaviour
    {
        public List<int> PlayerOrder = new List<int> { 1, 2 }; // Set PlayerIDs in Inspector
        
        private int _currentPlayerIndex = 0;
        private Dictionary<int, int> _jailTurns = new Dictionary<int, int>();
        private GameConfig _config;

        void Start()
        {
            _config = AppGame.Instance.Config;
            foreach(int pid in PlayerOrder)
            {
                _jailTurns[pid] = 0;
            }
            
            // Wait for AppGame to be ready
            Invoke("StartFirstTurn", 0.5f);
        }

        void OnEnable()
        {
            GameEvents.OnPlayerJailed += HandlePlayerJailed;
        }

        void OnDisable()
        {
            GameEvents.OnPlayerJailed -= HandlePlayerJailed;
        }

        void HandlePlayerJailed(int playerID)
        {
            _jailTurns[playerID] = _config.MaxJailTurns;
        }

        public int GetCurrentPlayerID()
        {
            return PlayerOrder[_currentPlayerIndex];
        }

        void StartFirstTurn()
        {
            AppGame.Instance.StateMachine.SetState(GameState.WaitingForRoll);
            GameEvents.RaiseTurnStarted(GetCurrentPlayerID());
        }

        public void RollDice()
        {
            if (AppGame.Instance.StateMachine.CurrentState != GameState.WaitingForRoll) return;

            int playerID = GetCurrentPlayerID();
            int die1 = Random.Range(1, 7);
            int die2 = Random.Range(1, 7);
            bool isDoubles = die1 == die2;

            GameEvents.RaiseDiceRolled(playerID, die1, die2);
            
            if (_jailTurns[playerID] > 0)
            {
                HandleJailTurn(playerID, isDoubles);
            }
            else
            {
                HandleNormalTurn(playerID, isDoubles);
            }
        }

        void HandleNormalTurn(int playerID, bool isDoubles)
        {
            // In an all-ImageTarget game, we don't move a pawn.
            // We just wait for the player to physically move their token.
            AppGame.Instance.StateMachine.SetState(GameState.WaitingForProximity);
            GameEvents.RaiseNotify($"Player {playerID}, move your token.");

            // If doubles rule is off, or if they didn't roll doubles, end the turn.
            // The RuleEngine will advance the state again once landing is resolved.
            if (!_config.AllowDoublesRule || !isDoubles)
            {
                // We don't advance the turn *yet*. We wait for the RuleEngine.
                // But we flag that this roll will not grant an extra turn.
            }
            else
            {
                GameEvents.RaiseNotify($"Player {playerID}, you rolled doubles! Roll again after your turn.");
                // We would set a flag here: _doublesRolled = true;
            }
        }

        void HandleJailTurn(int playerID, bool isDoubles)
        {
            if (isDoubles)
            {
                _jailTurns[playerID] = 0;
                GameEvents.RaisePlayerReleasedFromJail(playerID);
                GameEvents.RaiseNotify($"Player {playerID} rolled doubles and is out of jail!");
                HandleNormalTurn(playerID, false); // No extra turn on jail-release doubles
            }
            else
            {
                _jailTurns[playerID]--;
                if (_jailTurns[playerID] <= 0)
                {
                    // TODO: Force payment
                    AppGame.Instance.Bank.Pay(AppGame.Instance.GetWallet(playerID), _config.JailFine);
                    GameEvents.RaisePlayerReleasedFromJail(playerID);
                    GameEvents.RaiseNotify($"Player {playerID} paid {_config.JailFine} and is out of jail.");
                    HandleNormalTurn(playerID, false);
                }
                else
                {
                    GameEvents.RaiseNotify($"Player {playerID}, you are still in jail for {_jailTurns[playerID]} turns.");
                    AdvanceToNextPlayer();
                }
            }
        }

        // This is called by the RuleEngine after a turn is fully resolved
        public void AdvanceToNextPlayer()
        {
            GameEvents.RaiseTurnEnded(GetCurrentPlayerID());
            _currentPlayerIndex = (_currentPlayerIndex + 1) % PlayerOrder.Count;
            AppGame.Instance.StateMachine.SetState(GameState.WaitingForRoll);
            GameEvents.RaiseTurnStarted(GetCurrentPlayerID());
        }
    }
}
