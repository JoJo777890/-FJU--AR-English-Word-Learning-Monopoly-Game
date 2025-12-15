using ARMonopoly_V5___Full_Scale_V2.Board;
using ARMonopoly_V5___Full_Scale_V2.Core;
using ARMonopoly_V5___Full_Scale_V2.Data;
using ARMonopoly_V5___Full_Scale_V2.Economy;
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
        
        // (Tip): The property the player starts on (e.g., 'Go')
        public PropertyDef StartingProperty;

        [Header("Runtime")]
        // (Tip): The player's current logical index on the board (0 = Go).
        public int CurrentBoardIndex = 0;

        /// <summary>
        /// Called by AppGame to inject dependencies and initialize the player.
        /// </summary>
        public void Construct(Bank bank, GameConfig config, BoardDefinition board)
        {
            // 1. Register this player with the Bank
            bank.RegisterPlayer(PlayerID, config.StartingMoney);
            Debug.Log($"PlayerTag {PlayerID} initialized and registered.");

            // 2. Set Starting Position
            if (StartingProperty != null)
            {
                int startIndex = board.GetIndexFromID(StartingProperty.PropertyID);
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