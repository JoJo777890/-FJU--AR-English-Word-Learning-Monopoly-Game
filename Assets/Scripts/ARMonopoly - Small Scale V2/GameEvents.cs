// Assets/Scripts/Simple/GameEvents.cs
using System;

namespace ARMonopoly.Simple
{
    public static class GameEvents
    {
        // AR/game flow
        public static event Action<string> DistancesUpdated; // simple debug text

        // Land & resolve
        public static event Action<PropertyLanded> PropertyLanded;   // from PlayerTokenTrigger
        public static event Action<BuyPrompt> BuyPrompt;             // from Rules -> UI
        public static event Action<BuyRequest> BuyRequested;         // from UI -> Rules

        // Outcomes
        public static event Action<PropertyBought> PropertyBought;   // from Rules
        public static event Action<RentPaid> RentPaid;               // from Rules
        public static event Action<MoneyChanged> MoneyChanged;       // from Rules/AppLite

        // Raisers
        public static void RaiseDistancesUpdated(string s)
        {
            DistancesUpdated?.Invoke(s);
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
    }

    // Payloads (POCOs, Unity-friendly)
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
}
