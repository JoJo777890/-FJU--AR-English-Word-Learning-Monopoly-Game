// Assets/Scripts/Simple/AppLite.cs
using UnityEngine;

namespace ARMonopoly.Simple
{
    public class AppLite : MonoBehaviour
    {
        private void Start()
        {
            // Seed money labels for any PlayerTag in scene
            var players = FindObjectsOfType<PlayerTag>();
            foreach (var p in players)
            {
                SimpleGame.RegisterPlayer(p);
                SimpleUI.RefreshMoney(p.playerId, p.money);
            }
        }
    }
}