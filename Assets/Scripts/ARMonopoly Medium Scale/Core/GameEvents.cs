using System;
using ARMonopoly_Medium_Scale.Data;

namespace ARMonopoly_Medium_Scale.Core
{
    public static class GameEvents
    {
        // AR/debug
        public static event Action<string> DistancesUpdated;

        // Turns & dice
        public static event Action<int> TurnStarted;             // playerId
        public static event Action<int,int,int> DiceRolled;      // pid, d1, d2
        public static event Action<int> PassedGo;                // pid
        public static event Action<int> TurnEnded;               // pid

        // Land & resolve
        public static event Action<PropertyLanded> PropertyLanded;
        public static event Action<BuyPrompt> BuyPrompt;         
        public static event Action<BuyRequest> BuyRequested;     

        // Cards
        public static event Action<CardDrawn> CardDrawn;         // UI popup
        public static event Action<DeckType> DrawCardRequested;  // from Rules

        // Results
        public static event Action<PropertyBought> PropertyBought;
        public static event Action<RentPaid> RentPaid;
        public static event Action<MoneyChanged> MoneyChanged;
        public static event Action<int> SentToJail;              // pid
        public static event Action<int> ReleasedFromJail;        // pid

        // Raisers
        public static void RaiseDistancesUpdated(string s)
        {
            DistancesUpdated?.Invoke(s);
        }
        public static void RaiseTurnStarted(int pid)
        {
            TurnStarted?.Invoke(pid);
        }
        public static void RaiseDiceRolled(int pid,int d1,int d2)
        {
            DiceRolled?.Invoke(pid,d1,d2);
        }
        public static void RaisePassedGo(int pid)
        {
            PassedGo?.Invoke(pid);
        }
        public static void RaiseTurnEnded(int pid)
        {
            TurnEnded?.Invoke(pid);
        }
        public static void RaisePropertyLanded(PropertyLanded e)
        {
            PropertyLanded?.Invoke(e);
        }
        public static void RaiseBuyPrompt(BuyPrompt e)
        {
            BuyPrompt?.Invoke(e);
        }
        public static void RaiseBuyRequested(BuyRequest e)
        {
            BuyRequested?.Invoke(e);
        }
        public static void RaiseCardDrawn(CardDrawn e)
        {
            CardDrawn?.Invoke(e);
        }
        public static void RaiseDrawCardRequested(DeckType t)
        {
            DrawCardRequested?.Invoke(t);
        }
        public static void RaisePropertyBought(PropertyBought e)
        {
            PropertyBought?.Invoke(e);
        }
        public static void RaiseRentPaid(RentPaid e)
        {
            RentPaid?.Invoke(e);
        }
        public static void RaiseMoneyChanged(MoneyChanged e)
        {
            MoneyChanged?.Invoke(e);
        }
        public static void RaiseSentToJail(int pid)
        {
            SentToJail?.Invoke(pid);
        }
        public static void RaiseReleasedFromJail(int pid)
        {
            ReleasedFromJail?.Invoke(pid);
        }
    }

    // Payloads
    public struct PropertyLanded 
    {
        public int playerId; 
        public string propertyId; 
        public string propertyName;
    }
    public struct BuyPrompt 
    {
        public int playerId; 
        public string propertyId; 
        public string propertyName; 
        public int price;
    }
    public struct BuyRequest 
    {
        public int playerId; 
        public string propertyId;
    }
    public struct PropertyBought 
    {
        public int playerId; 
        public string propertyId; 
        public string propertyName; 
        public int price;
    }
    public struct RentPaid 
    {
        public int payerId; 
        public int ownerId; 
        public string propertyId; 
        public string propertyName; 
        public int amount;
    }
    public struct MoneyChanged 
    {
        public int playerId; 
        public int money;
    }
    public struct CardDrawn 
    {
        public DeckType deck; 
        public string title; 
        public string body;
    }
}
