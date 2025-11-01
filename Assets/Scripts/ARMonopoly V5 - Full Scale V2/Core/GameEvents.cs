using System;

namespace ARMonopoly_V5___Full_Scale_V2.Core
{
    // This enum defines the central state of the game.
    // It is defined here so all other scripts can reference it.
    public enum GameState
    {
        Boot,
        PlayerTurn,         // Waiting for player to click "Roll"
        AwaitingPlayerMove, // Player has rolled, waiting for physical move
        ResolvingSpace      // Player has landed, RuleEngine is resolving (e.g., show Buy panel)
    }

    public static class GameEvents
    {
        // --- Game State ---
        public static event Action<GameState> OnStateChanged;
        public static void RaiseStateChanged(GameState newState) => OnStateChanged?.Invoke(newState);

        // --- Turn Flow ---
        public static event Action<int> OnTurnStarted;
        public static void RaiseTurnStarted(int playerID) => OnTurnStarted?.Invoke(playerID);

        public static event Action<int, int> OnDiceRolled;
        public static void RaiseDiceRolled(int playerID, int totalRoll) => OnDiceRolled?.Invoke(playerID, totalRoll);

        public static event Action<MovePayload> OnMoveRequired;
        public static void RaiseMoveRequired(MovePayload payload) => OnMoveRequired?.Invoke(payload);

        public static event Action<int> OnPlayerPassedGo;
        public static void RaisePlayerPassedGo(int playerID) => OnPlayerPassedGo?.Invoke(playerID);

        // --- Proximity ---
        public static event Action<ProximityPayload> OnProximityEnter;
        public static void RaiseProximityEnter(ProximityPayload payload) => OnProximityEnter?.Invoke(payload);

        public static event Action<ProximityPayload> OnProximityExit;
        public static void RaiseProximityExit(ProximityPayload payload) => OnProximityExit?.Invoke(payload);

        // --- Rules & Economy ---
        public static event Action<BuyPayload> OnBuyPrompt;
        public static void RaiseBuyPrompt(BuyPayload payload) => OnBuyPrompt?.Invoke(payload);

        public static event Action<string> OnBuyRequest;
        public static void RaiseBuyRequest(string propertyID) => OnBuyRequest?.Invoke(propertyID);

        public static event Action<PropertyPayload> OnPropertyBought;
        public static void RaisePropertyBought(PropertyPayload payload) => OnPropertyBought?.Invoke(payload);

        public static event Action<RentPayload> OnRentPaid;
        public static void RaiseRentPaid(RentPayload payload) => OnRentPaid?.Invoke(payload);

        public static event Action<int, int> OnMoneyChanged;
        public static void RaiseMoneyChanged(int playerID, int newBalance) => OnMoneyChanged?.Invoke(playerID, newBalance);
    }

    #region Event Payloads

    public struct ProximityPayload
    {
        public int PlayerID;
        public string PropertyID;
    }

    public struct MovePayload
    {
        public int PlayerID;
        public string DestinationName;
        public string DestinationPropertyID;
    }

    public struct BuyPayload
    {
        public int PlayerID;
        public string PropertyID;
        public string PropertyName;
        public int Price; // <-- FIXED: Was PricePrice
    }

    public struct PropertyPayload
    {
        public int PlayerID;
        public string PropertyID;
        public string PropertyName;
    }

    public struct RentPayload
    {
        public int PayerID;
        public int OwnerID;
        public string PropertyID;
        public string PropertyName;
        public int Amount;
    }

    #endregion
}

