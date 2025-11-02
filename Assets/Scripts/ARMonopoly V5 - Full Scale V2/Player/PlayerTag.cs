using ARMonopoly_V5___Full_Scale_V2.Core;
using ARMonopoly_V5___Full_Scale_V2.Data;
using UnityEngine;

namespace ARMonopoly_V5___Full_Scale_V2.Player
{
    /// <summary>
    /// Attached to the Player's Image Target.
    /// Identifies the player and holds their logical board position.
    /// </summary>
    public class PlayerTag : MonoBehaviour
    {
        [Header("Config")]
        public int PlayerID;
        public string PlayerName;

        [Tooltip("The property the player starts on (e.g., 'Go')")]
        public PropertyDef StartingProperty;

        [Header("Runtime")]
        [Tooltip("The player's current logical index on the board.")]
        public int CurrentBoardIndex = 0;

        private void Start()
        {
            if (AppGame.Instance == null || AppGame.Instance.Board == null || AppGame.Instance.Config == null)
            {
                Debug.LogError($"PlayerTag {PlayerID}: AppGame or its assets are not ready!", this);
                return;
            }
            
            // --- 1. REGISTER WITH BANK ---
            if (AppGame.Instance.Bank != null)
            {
                AppGame.Instance.Bank.RegisterPlayer(PlayerID, AppGame.Instance.Config.StartingMoney);
                Debug.Log($"PlayerTag {PlayerID} registered with Bank.");
            }
            else
            {
                Debug.LogError($"PlayerTag {PlayerID}: Could not find Bank to register with!", this);
            }

            // --- 2. REGISTER WITH INVESTMENT SERVICE ---
            if (AppGame.Instance.Investments != null)
            {
                AppGame.Instance.Investments.RegisterPlayer(PlayerID);
                Debug.Log($"PlayerTag {PlayerID} registered with InvestmentService.");
            }
            
            // --- 3. SET STARTING POSITION ---
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
