using ARMonopoly_Medium_Scale.Core;
using ARMonopoly_Medium_Scale.Data;
using ARMonopoly_Medium_Scale.Services;
using UnityEngine;

namespace ARMonopoly_Medium_Scale.Gameplay
{
    public class TurnController : MonoBehaviour
    {
        public int[] playerOrder = {0,1}; // set based on how many tokens you have
        private int _turnIndex = 0;
        private AppGame _app;
        private GameConfig _cfg;
        private int[] _jailedTurns; // simple jail tracker

        private void Awake()
        {
            _app = FindObjectOfType<AppGame>();
            _cfg = _app.gameConfig;
            _jailedTurns = new int[4];
        }

        private void Start()
        {
            StartTurn();
        }

        public void StartTurn()
        {
            int pid = playerOrder[_turnIndex % playerOrder.Length];
            _app.SetCurrentPlayer(pid);
            GameEvents.RaiseTurnStarted(pid);
        }

        public void RollDice()
        {
            int pid = _app.CurrentPlayerId;
            int d1 = Random.Range(1, 7);
            int d2 = Random.Range(1, 7);
            GameEvents.RaiseDiceRolled(pid, d1, d2);

            // Jail handling (super minimal)
            if (_jailedTurns[pid] > 0)
            {
                // Pay fine to get out this turn (simplified)
                FindService<EconomyService>().Debit(pid, _cfg.jailFine);
                _jailedTurns[pid] = 0;
                GameEvents.RaiseReleasedFromJail(pid);
            }

            // If you want GO/pass/board movement, you need a board index. In all-ImageTarget mode
            // we let landing be determined by PlayerTokenTrigger proximity. So here we just end turn.
            bool doubles = d1 == d2;
            if (_cfg.useDoublesExtraTurn && doubles)
            {
                // grant another roll (no turn advance)
                return;
            }

            EndTurn();
        }

        public void SendToJail(int pid)
        {
            _jailedTurns[pid] = _cfg.maxJailTurns; 
            GameEvents.RaiseSentToJail(pid);
        }

        public void EndTurn()
        {
            int pid = _app.CurrentPlayerId;
            GameEvents.RaiseTurnEnded(pid);
            _turnIndex++;
            StartTurn();
        }

        private T FindService<T>() where T : class => _app.GetService<T>();
    }
}
