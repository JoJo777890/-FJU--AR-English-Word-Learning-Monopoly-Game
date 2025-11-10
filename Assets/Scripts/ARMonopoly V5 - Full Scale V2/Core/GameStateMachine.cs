using UnityEngine;

namespace ARMonopoly_V5___Full_Scale_V2.Core
{
    /// <summary>
    /// A lightweight service (not a MonoBehaviour) that holds the single source
    /// of truth for the game's current state.
    /// Instantiated by AppGame.
    /// </summary>
    public class GameStateMachine
    {
        /// <summary>
        /// The current state of the game.
        /// </summary>
        public GameState CurrentState { get; private set; }

        public GameStateMachine()
        {
            CurrentState = GameState.Boot;
        }

        /// <summary>
        /// Sets the new game state and raises the OnStateChanged event.
        /// </summary>
        /// <param name="newState">The state to transition to.</param>
        public void SetState(GameState newState)
        {
            if (CurrentState == newState)
                return;

            CurrentState = newState;
            Debug.Log($"[GameStateMachine] New State: {newState}");

            // Notify all listeners
            GameEvents.RaiseStateChanged(newState);
        }
    }
}