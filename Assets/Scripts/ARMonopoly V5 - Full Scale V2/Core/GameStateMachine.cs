namespace ARMonopoly_V5___Full_Scale_V2.Core
{
    public enum GameState
    {
        Initializing,
        WaitingForPlayers,
        PlayerTurn,
        WaitingForRoll,
        WaitingForProximity,
        ResolvingTurn,
        GameOver
    }

    public class GameStateMachine
    {
        public GameState CurrentState { get; private set; }

        public GameStateMachine()
        {
            CurrentState = GameState.Initializing;
        }

        public void SetState(GameState newState)
        {
            if (CurrentState == newState) return;

            GameState oldState = CurrentState;
            CurrentState = newState;
            
            GameEvents.RaiseGameStateChanged(oldState, newState);
        }
    }
}