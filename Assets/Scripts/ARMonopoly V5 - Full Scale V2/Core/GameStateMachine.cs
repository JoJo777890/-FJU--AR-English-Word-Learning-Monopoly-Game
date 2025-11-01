using ARMonopoly_V5___Full_Scale_V2.Core;
using UnityEngine;

namespace ARMonopoly_V5___Full_Scale_V2.Core
{
    /// <summary>
    /// A service class (not a MonoBehaviour) that holds the one true game state.
    /// Instantiated and held by AppGame.
    /// </summary>
    public class GameStateMachine
    {
        public GameState CurrentState { get; private set; }

        public GameStateMachine()
        {
            CurrentState = GameState.Boot;
        }

        /// <summary>
        /// Sets the new game state and notifies all listeners.
        /// </summary>
        public void SetState(GameState newState)
        {
            if (CurrentState == newState)
                return;

            CurrentState = newState;
            Debug.Log($"[GameStateMachine] New State: {newState}");
            
            // Fire the global event
            GameEvents.RaiseStateChanged(newState);
        }
    }
}