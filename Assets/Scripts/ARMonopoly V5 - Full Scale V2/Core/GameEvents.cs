using System;
using ARMonopoly_V5___Full_Scale_V2.Data;

namespace ARMonopoly_V5___Full_Scale_V2.Core
{
    // --- GAME STATE ---
    public static class GameEvents
    {
        public static event Action<GameState, GameState> OnGameStateChanged;
        public static void RaiseGameStateChanged(GameState from, GameState to) => OnGameStateChanged?.Invoke(from, to);

        // --- TURN FLOW ---
        public static event Action<int> OnTurnStarted; // PlayerID
        public static event Action<int, int, int> OnDiceRolled; // PlayerID, Die1, Die2
        public static event Action<int> OnTurnEnded; // PlayerID
        public static event Action<int> OnPlayerJailed; // PlayerID
        public static event Action<int> OnPlayerReleasedFromJail; // PlayerID

        public static void RaiseTurnStarted(int pid) => OnTurnStarted?.Invoke(pid);
        public static void RaiseDiceRolled(int pid, int d1, int d2) => OnDiceRolled?.Invoke(pid, d1, d2);
        public static void RaiseTurnEnded(int pid) => OnTurnEnded?.Invoke(pid);
        public static void RaisePlayerJailed(int pid) => OnPlayerJailed?.Invoke(pid);
        public static void RaisePlayerReleasedFromJail(int pid) => OnPlayerReleasedFromJail?.Invoke(pid);

        // --- AR INTERACTION ---
        public static event Action<ProximityPayload> OnProximityEnter; // Player landed on property
        public static event Action<ProximityPayload> OnProximityExit; // Player left property
        public static void RaiseProximityEnter(ProximityPayload p) => OnProximityEnter?.Invoke(p);
        public static void RaiseProximityExit(ProximityPayload p) => OnProximityExit?.Invoke(p);


        // --- UI & RULES (REQUESTS) ---
        public static event Action<BuyRequestPayload> OnBuyRequested; // UI -> Rules
        public static void RaiseBuyRequested(BuyRequestPayload p) => OnBuyRequested?.Invoke(p);

        // --- RULES & UI (PROMPTS) ---
        public static event Action<BuyPromptPayload> OnBuyPrompt; // Rules -> UI
        public static event Action<string> OnNotify; // Rules -> UI (e.g., "You paid $50 rent")
        public static void RaiseBuyPrompt(BuyPromptPayload p) => OnBuyPrompt?.Invoke(p);
        public static void RaiseNotify(string message) => OnNotify?.Invoke(message);

        // --- ECONOMY ---
        public static event Action<int, int> OnMoneyChanged; // PlayerID, NewBalance
        public static event Action<PropertyBoughtPayload> OnPropertyBought; // Rules -> All
        public static void RaiseMoneyChanged(int pid, int newBalance) => OnMoneyChanged?.Invoke(pid, newBalance);
        public static void RaisePropertyBought(PropertyBoughtPayload p) => OnPropertyBought?.Invoke(p);
    }

    // --- Event Payloads ---
    
    public struct ProximityPayload
    {
        public int PlayerID;
        public string PropertyID;
        public PropertyDef PropertyDef;
    }

    public struct BuyPromptPayload
    {
        public int PlayerID;
        public string PropertyID;
        public string DisplayName;
        public int Price;
    }

    public struct BuyRequestPayload
    {
        public int PlayerID;
        public string PropertyID;
    }

    public struct PropertyBoughtPayload
    {
        public int PlayerID;
        public string PropertyID;
        public int Price;
    }
}
