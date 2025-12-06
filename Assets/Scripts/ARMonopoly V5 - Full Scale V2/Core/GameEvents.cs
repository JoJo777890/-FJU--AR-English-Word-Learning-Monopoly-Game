using System;
using ARMonopoly_V5___Full_Scale_V2.Data; // Required for SpellingQuestion payload

namespace ARMonopoly_V5___Full_Scale_V2.Core
{
    /// <summary>
    /// Defines the primary game flow states.
    /// </summary>
    public enum GameState
    {
        /// <summary>
        /// Initial state on game launch.
        /// </summary>
        Boot,
        
        /// <summary>
        /// Waiting for the current player to roll the dice.
        /// </summary>
        PlayerTurn,
        
        /// <summary>
        /// Player has rolled, waiting for their physical token to move to the destination.
        /// </summary>
        AwaitingPlayerMove,
        
        /// <summary>
        /// Player has landed, RuleEngine is resolving the space (e.g., Buy panel).
        /// </summary>
        ResolvingSpace,
        
        /// <summary>
        /// Player has landed, waiting for spelling/investment/answer.
        /// </summary>
        AwaitingSpellingAnswer,
        
        /// <summary>
        /// Player has answered, RuleEngine is resolving spelling results.
        /// </summary>
        ResolvingSpelling
    }

    /// <summary>
    /// Static event bus for decoupled, game-wide communication.
    /// </summary>
    public static class GameEvents
    {
        // --- Game State ---
        
        /// <summary>
        /// Fires when the global GameState changes.
        /// </summary>
        public static event Action<GameState> OnStateChanged;
        public static void RaiseStateChanged(GameState newState) => OnStateChanged?.Invoke(newState);

        // --- Log Event ---
        
        /// <summary>
        /// Fires to request a message be displayed in the UI log.
        /// </summary>
        public static event Action<string> OnLogMessage;
        public static void RaiseLogMessage(string message) => OnLogMessage?.Invoke(message);

        // --- Turn Flow ---
        
        /// <summary>
        /// Fires when a new player's turn begins.
        /// </summary>
        public static event Action<int> OnTurnStarted;
        public static void RaiseTurnStarted(int playerID) => OnTurnStarted?.Invoke(playerID);

        /// <summary>
        /// Fires when the dice are rolled.
        /// </summary>
        public static event Action<int, int> OnDiceRolled;
        public static void RaiseDiceRolled(int playerID, int totalRoll) => OnDiceRolled?.Invoke(playerID, totalRoll);

        /// <summary>
        /// Fires when a move is required to a new destination.
        /// </summary>
        public static event Action<MovePayload> OnMoveRequired;
        public static void RaiseMoveRequired(MovePayload payload) => OnMoveRequired?.Invoke(payload);

        /// <summary>
        /// Fires when a player's move calculation passes 'Go'.
        /// </summary>
        public static event Action<int> OnPlayerPassedGo;
        public static void RaisePlayerPassedGo(int playerID) => OnPlayerPassedGo?.Invoke(playerID);

        // --- Proximity ---
        
        /// <summary>
        /// Fires when a PlayerTokenTrigger enters proximity of a PropertyTag.
        /// </summary>
        public static event Action<ProximityPayload> OnProximityEnter;
        public static void RaiseProximityEnter(ProximityPayload payload) => OnProximityEnter?.Invoke(payload);

        /// <summary>
        /// Fires when a PlayerTokenTrigger exits proximity of a PropertyTag.
        /// </summary>
        public static event Action<ProximityPayload> OnProximityExit;
        public static void RaiseProximityExit(ProximityPayload payload) => OnProximityExit?.Invoke(payload);
        
        // --- Spelling & Investment ---
        
        /// <summary>
        /// Fires to initiate a spelling challenge.
        /// </summary>
        public static event Action<SpellingQuestionPayload> OnSpellingQuestion;
        public static void RaiseSpellingQuestion(SpellingQuestionPayload payload) => OnSpellingQuestion?.Invoke(payload);

        /// <summary>
        /// Fires when a player (not the current one) invests in the spelling answer.
        /// </summary>
        public static event Action<InvestmentPayload> OnPlayerInvest;
        public static void RaisePlayerInvest(InvestmentPayload payload) => OnPlayerInvest?.Invoke(payload);

        /// <summary>
        /// Fires when the current player submits their spelling answer.
        /// </summary>
        public static event Action<SpellingAnswerPayload> OnSpellingAnswer;
        public static void RaiseSpellingAnswer(SpellingAnswerPayload payload) => OnSpellingAnswer?.Invoke(payload);


        // --- Rules & Economy ---
        
        /// <summary>
        /// Fires when the RuleEngine determines a property is available for purchase.
        /// </summary>
        public static event Action<BuyPayload> OnBuyPrompt;
        public static void RaiseBuyPrompt(BuyPayload payload) => OnBuyPrompt?.Invoke(payload);

        /// <summary>
        /// Fires when the UI confirms a purchase request.
        /// </summary>
        public static event Action<string> OnBuyRequest;
        public static void RaiseBuyRequest(string propertyID) => OnBuyRequest?.Invoke(propertyID);

        /// <summary>
        /// Fires when a property purchase is successfully processed by the Bank.
        /// </summary>
        public static event Action<PropertyPayload> OnPropertyBought;
        public static void RaisePropertyBought(PropertyPayload payload) => OnPropertyBought?.Invoke(payload);

        /// <summary>
        /// Fires when rent is successfully transferred.
        /// </summary>
        public static event Action<RentPayload> OnRentPaid;
        public static void RaiseRentPaid(RentPayload payload) => OnRentPaid?.Invoke(payload);

        /// <summary>
        /// Fires when a player's wallet balance changes.
        /// </summary>
        public static event Action<int, int> OnMoneyChanged;
        public static void RaiseMoneyChanged(int playerID, int newBalance) => OnMoneyChanged?.Invoke(playerID, newBalance);
    }

    #region Event Payloads

    /// <summary>
    /// Payload for proximity events (enter/exit).
    /// </summary>
    public struct ProximityPayload 
    {
        public int PlayerID; 
        public string PropertyID;
    }
    
    /// <summary>
    /// Payload instructing the UI to wait for a physical move.
    /// </summary>
    public struct MovePayload 
    {
        public int PlayerID; 
        public string DestinationName; 
        public string DestinationPropertyID;
    }
    
    /// <summary>
    /// Payload carrying information for a buy prompt.
    /// </summary>
    public struct BuyPayload 
    {
        public int PlayerID; 
        public string PropertyID; 
        public string PropertyName; 
        public int Price;
    }
    
    /// <summary>
    /// Payload confirming a successful property purchase.
    /// </summary>
    public struct PropertyPayload 
    {
        public int PlayerID; 
        public string PropertyID; 
        public string PropertyName; 
        public int Price;
    }
    
    /// <summary>
    /// Payload confirming a rent payment.
    /// </summary>
    public struct RentPayload 
    {
        public int PayerID; 
        public int OwnerID; 
        public string PropertyID; 
        public string PropertyName; 
        public int Amount;
    }
    
    /// <summary>
    /// Payload carrying the details of a new spelling challenge.
    /// </summary>
    public struct SpellingQuestionPayload 
    {
        public int PlayerID; 
        public SpellingQuestion Question; 
        public string PropertyID;
    }
    
    /// <summary>
    /// Payload for a player's investment action.
    /// </summary>
    public struct InvestmentPayload 
    {
        public int InvestorID; 
        public int TargetPlayerID; 
        public int Amount;
    }
    
    /// <summary>
    /// Payload carrying the player's submitted answer.
    /// </summary>
    public struct SpellingAnswerPayload { 
        public int PlayerID; 
        public string Answer; 
    }
    
    #endregion
}