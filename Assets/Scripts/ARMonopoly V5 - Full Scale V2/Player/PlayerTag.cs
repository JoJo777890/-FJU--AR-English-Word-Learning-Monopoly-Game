using UnityEngine;

namespace ARMonopoly_V5___Full_Scale_V2.Player
{
    /// <summary>
    /// Attach to each Player ImageTarget.
    /// Must also have a Wallet component.
    /// </summary>
    [RequireComponent(typeof(ARMonopoly_V5___Full_Scale_V2.Economy.Wallet))]
    public class PlayerTag : MonoBehaviour
    {
        public int PlayerID = 1; // 1, 2, 3, 4 etc.
        public string PlayerName = "Player 1";
    }
}