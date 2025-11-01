using System;
using ARMonopoly_V5___Full_Scale_V2.Data;

namespace ARMonopoly_V5___Full_Scale_V2.Core
{
    // UPDATED GameState Enum
    public enum GameState
    {
        Boot,
        PlayerTurn,
        AwaitingPlayerMove,
        ResolvingSpace,
        AwaitingSpelling,  // Player needs to answer, other players can invest
        ResolvingSpelling  // Answer submitted, checking...
    }

    // --- PAYLOADS ---
    
    // Fired by TurnController when a move is calculated
    public struct MovePayload
    {
        public int PlayerID;
        public string DestinationName;
        public string DestinationPropertyID;
    }
    
    // Fired by PlayerTokenTrigger
    public struct ProximityPayload
    {
        public int PlayerID;
        public string PropertyID;
    }

    // Fired by RuleEngine to ask UI to show Buy Panel
    public struct BuyPayload
    {
        public int PlayerID;
        public string PropertyID;
        public string PropertyName;
        public int Price;
    }

    // Fired by RuleEngine when rent is successfully paid
    public struct RentPayload
    {
        public int PayerID;
        public int OwnerID;
        public string PropertyID;
        public string PropertyName;
        public int Amount;
    }
    
    // Fired by RuleEngine when a property is successfully bought
    public struct PropertyPayload
    {
        public int PlayerID;
        public string PropertyID;
        public string PropertyName;
        public int Price;
    }
    
    // Fired by RuleEngine to start a quiz
    public struct SpellingQuizPayload
    {
        public int PlayerID;
        public PropertyDef Property;
        public SpellingQuestion Question;
        public bool IsBuyQuiz; // true = buying, false = waiving rent
    }

    // Fired by UI to check the player's physical answer
    public struct CheckAnswerRequest
    {
        public int PlayerID;
    }

    // Fired by UI for another player to invest
    public struct InvestRequest
    {
        public int PlayerID; // The player *making* the investment
    }
    
    // Fired to show quiz results
    public struct QuizResultPayload
    {
        public string Title;
        public string Message;
    }

    public static class GameEvents
    {
        // --- State ---
        public static event Action<GameState> OnStateChanged;
        public static void RaiseStateChanged(GameState newState) => OnStateChanged?.Invoke(newState);

        // --- Turn Flow ---
        public static event Action<int> OnTurnStarted;
        public static void RaiseTurnStarted(int playerID) => OnTurnStarted?.Invoke(playerID);
        
        public static event Action<int, int> OnDiceRolled;
        public static void RaiseDiceRolled(int playerID, int roll) => OnDiceRolled?.Invoke(playerID, roll);
        
        public static event Action<MovePayload> OnMoveRequired;
        public static void RaiseMoveRequired(MovePayload payload) => OnMoveRequired?.Invoke(payload);
        
        public static event Action<int> OnPlayerPassedGo;
        public static void RaisePlayerPassedGo(int playerID) => OnPlayerPassedGo?.Invoke(playerID);

        // --- Proximity ---
        public static event Action<ProximityPayload> OnProximityEnter;
        public static void RaiseProximityEnter(ProximityPayload payload) => OnProximityEnter?.Invoke(payload);

        public static event Action<ProximityPayload> OnProximityExit;
        public static void RaiseProximityExit(ProximityPayload payload) => OnProximityExit?.Invoke(payload);
        
        // --- Spelling Quiz & Investment ---
        public static event Action<SpellingQuizPayload> OnSpellingQuizStarted;
        public static void RaiseSpellingQuizStarted(SpellingQuizPayload payload) => OnSpellingQuizStarted?.Invoke(payload);

        public static event Action<InvestRequest> OnInvestRequest;
        public static void RaiseInvestRequest(InvestRequest payload) => OnInvestRequest?.Invoke(payload);

        public static event Action<int, string, int> OnPlayerInvested;
        public static void RaisePlayerInvested(int playerID, string propertyName, int amount) => OnPlayerInvested?.Invoke(playerID, propertyName, amount);

        public static event Action<CheckAnswerRequest> OnCheckAnswerRequest;
        public static void RaiseCheckAnswerRequest(CheckAnswerRequest payload) => OnCheckAnswerRequest?.Invoke(payload);

        public static event Action<QuizResultPayload> OnQuizResult;
        public static void RaiseQuizResult(string title, string message) => OnQuizResult?.Invoke(new QuizResultPayload { Title = title, Message = message });
        
        // --- Resolution ---
        public static event Action<BuyPayload> OnBuyPrompt;
        public static void RaiseBuyPrompt(BuyPayload payload) => OnBuyPrompt?.Invoke(payload);

        public static event Action<string> OnBuyRequest;
        public static void RaiseBuyRequest(string propertyID) => OnBuyRequest?.Invoke(propertyID);
        
        public static event Action<string> OnPassRequest;
        public static void RaisePassRequest(string propertyID) => OnPassRequest?.Invoke(propertyID);

        public static event Action<PropertyPayload> OnPropertyBought;
        public static void RaisePropertyBought(PropertyPayload payload) => OnPropertyBought?.Invoke(payload);

        public static event Action<RentPayload> OnRentPaid;
        public static void RaiseRentPaid(RentPayload payload) => OnRentPaid?.Invoke(payload);

        public static event Action<int, int> OnMoneyChanged;
        public static void RaiseMoneyChanged(int playerID, int newBalance) => OnMoneyChanged?.Invoke(playerID, newBalance);
    }
}

