// Assets/Scripts/Simple/AppLite.cs
using UnityEngine;

namespace ARMonopoly.Simple
{
    public class AppLite : MonoBehaviour
    {
        private void Start()
        {
            foreach (var p in GameObject.FindObjectsOfType<PlayerTag>())
                SimpleGame.RegisterPlayer(p);
        }
    }
}