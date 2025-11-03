// In folder: ARMonopoly V5 - Full Scale V2/Core/
using System;

namespace ARMonopoly_V5___Full_Scale_V2.Core
{
    public enum GameState
    {
        Boot,
        PlayerTurn,
        AwaitingPlayerMove,
        ResolvingSpace,
        AwaitingSpellingAnswer, // New State
        ResolvingSpelling      // New State
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
        
        // --- Spelling & Investment ---
        public static event Action<SpellingQuestionPayload> OnSpellingQuestion;
        public static void RaiseSpellingQuestion(SpellingQuestionPayload payload) => OnSpellingQuestion?.Invoke(payload);

        public static event Action<InvestmentPayload> OnPlayerInvest;
        public static void RaisePlayerInvest(InvestmentPayload payload) => OnPlayerInvest?.Invoke(payload);

        public static event Action<SpellingAnswerPayload> OnSpellingAnswer;
        public static void RaiseSpellingAnswer(SpellingAnswerPayload payload) => OnSpellingAnswer?.Invoke(payload);


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
        public int Price;
    }
    public struct PropertyPayload 
    {
        public int PlayerID; 
        public string PropertyID; 
        public string PropertyName; 
        public int Price;
    }
    public struct RentPayload 
    {
        public int PayerID; 
        public int OwnerID; 
        public string PropertyID; 
        public string PropertyName; 
        public int Amount;
    }
    
    // --- New Payloads ---
    public struct SpellingQuestionPayload 
    {
        public int PlayerID; 
        public Data.SpellingQuestion Question; 
        public string PropertyID;
    }
    public struct InvestmentPayload 
    {
        public int InvestorID; 
        public int TargetPlayerID; 
        public int Amount;
    }
    public struct SpellingAnswerPayload { 
        public int PlayerID; 
        public string Answer; 
    }


    #endregion
}