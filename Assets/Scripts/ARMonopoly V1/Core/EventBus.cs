using System;
using UnityEngine;

namespace ARMonopoly.Core
{
    public static class GameEvents // "App.cs" contains "class GameEvents" as well. (Fix Later)
    {
        public static event Action<int> TurnStarted;                    // playerId
        public static event Action<int, int> DiceRolled;                // playerId, total
        public static event Action<int, int> LandedOn;                  // playerId, spaceIndex
        // public static event Action<string, bool> ArTargetChanged;    // targetName, isTracked
        
        // and anything else...
        // e.g., PropertyPurchased, TurnAdvanced, ...
    }
}


