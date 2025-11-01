using UnityEngine;

namespace ARMonopoly_V5___Full_Scale_V2.Core
{
    /// <summary>
    /// Manages the one true GameState.
    /// It uses the 'GameState' enum defined in GameEvents.cs.
    /// </summary>
    public class GameStateMachine
    {
        public GameState CurrentState { get; private set; }

        public GameStateMachine()
        {
            CurrentState = GameState.Boot;
        }

        public void SetState(GameState newState)
        {
            if (CurrentState == newState)
                return;

            CurrentState = newState;
            Debug.Log($"[GameStateMachine] New State: {newState}");

            // Notify all listeners that the state has changed
            GameEvents.RaiseStateChanged(newState);
        }
    }
}