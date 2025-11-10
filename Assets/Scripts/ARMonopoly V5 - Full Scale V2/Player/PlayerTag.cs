using ARMonopoly_V5___Full_Scale_V2.Core;
using ARMonopoly_V5___Full_Scale_V2.Data;
using UnityEngine;

namespace ARMonopoly_V5___Full_Scale_V2.Player
{
    /// <summary>
    /// Identifies a player token in the scene (attached to the Player's Image Target)
    /// and holds their logical board position.
    /// </summary>
    public class PlayerTag : MonoBehaviour
    {
        [Header("Config")]
        public int PlayerID;
        public string PlayerName;

        [Tooltip("The property the player starts on (e.g., 'Go')")]
        public PropertyDef StartingProperty;

        [Header("Runtime")]
        [Tooltip("The player's current logical index on the board (0 = Go).")]
        public int CurrentBoardIndex = 0;

        private void Start()
        {
            if (AppGame.Instance == null || AppGame.Instance.Board == null || AppGame.Instance.Config == null)
            {
                Debug.LogError($"PlayerTag {PlayerID}: AppGame or its assets are not ready!", this);
                return;
            }
            
            // 1. Register this player with the Bank
            // This also triggers the initial OnMoneyChanged event for the UI.
            if (AppGame.Instance.Bank != null)
            {
                AppGame.Instance.Bank.RegisterPlayer(PlayerID, AppGame.Instance.Config.StartingMoney);
                Debug.Log($"PlayerTag {PlayerID} registered with Bank.");
            }
            else
            {
                Debug.LogError($"PlayerTag {PlayerID}: Could not find Bank to register with!", this);
            }
            
            // 2. Set the logical starting position on the board
            if (StartingProperty != null)
            {
                int startIndex = AppGame.Instance.Board.GetIndexFromID(StartingProperty.PropertyID);
                if (startIndex != -1)
                {
                    CurrentBoardIndex = startIndex;
                }
                else
                {
                    Debug.LogWarning($"Could not find StartingProperty '{StartingProperty.DisplayName}' in BoardDefinition. Defaulting to index 0.");
                    CurrentBoardIndex = 0;
                }
            }
            else
            {
                Debug.LogWarning($"Player {PlayerID} has no StartingProperty assigned. Defaulting to index 0.");
                CurrentBoardIndex = 0;
            }
        }
    }
}