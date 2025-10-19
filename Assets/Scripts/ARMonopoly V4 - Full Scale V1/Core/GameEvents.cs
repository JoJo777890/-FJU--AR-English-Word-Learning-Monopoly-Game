using System;

namespace ARMonopoly_V4___Full_Scale_V1.Core
{
    public static class GameEvents
    {
        // AR/debug
        public static event Action<string> DistancesUpdated;

        // Core
        public static event Action GameStarted;                  // 
        
        // Turns & dice
        public static event Action<int> TurnStarted;             // playerId
        public static event Action<int,int,int> DiceRolled;      // pid, d1, d2
        public static event Action<int> PassedGo;                // pid
        public static event Action<int> TurnEnded;               // pid

        // Land & resolve
        public static event Action<PropertyLanded> PropertyLanded;
        public static event Action<BuyPrompt> BuyPrompt;         
        public static event Action<BuyRequest> BuyRequested;     

        // // Cards
        // public static event Action<CardDrawn> CardDrawn;         // UI popup
        // public static event Action<DeckType> DrawCardRequested;  // from Rules

        // Results
        public static event Action<PropertyBought> PropertyBought;
        public static event Action<RentPaid> RentPaid;
        public static event Action<MoneyChanged> MoneyChanged;
        public static event Action<int> SentToJail;              // pid
        public static event Action<int> ReleasedFromJail;        // pid

        //GameStates
        public static event Action GameStart;
        public static event Action TurnStart;
        public static event Action RollDice;
        public static event Action WaitForPlayerToMoveToken;
        public static event Action PlayerMovedToken;
        public static event Action PayRent;
        public static event Action BuyProperty;
        public static event Action TurnEnd;
        public static event Action GameEnd;
        
        // Raisers
        public static void RaiseGameStart()
        {
            GameStart?.Invoke();
        }
        public static void RaiseTurnStart()
        {
            TurnStart?.Invoke();
        }
        public static void RaiseRollDice()
        {
            RollDice?.Invoke();
        }
        public static void RaiseWaitForPlayerToMoveToken()
        {
            WaitForPlayerToMoveToken?.Invoke();
        }
        public static void RaiseplayerMovedToken()
        {
            PlayerMovedToken?.Invoke();
        }
        public static void RaisePayRent()
        {
            PayRent?.Invoke();
        }
        public static void RaiseBuyProperty()
        {
            BuyProperty?.Invoke();
        }
        public static void RaiseTurnEnd()
        {
            TurnEnd?.Invoke();
        }
        public static void RaiseGameEnd()
        {
            GameEnd?.Invoke();
        }
        //
        public static void RaiseDistancesUpdated(string s)
        {
            DistancesUpdated?.Invoke(s);
        }
        public static void RaiseGameStarted()
        {
            GameStarted?.Invoke();
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
        public static void RaisePropertyLanded(PropertyLanded e) // All "e" means "event argument"
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
        // public static void RaiseCardDrawn(CardDrawn e)
        // {
        //     CardDrawn?.Invoke(e);
        // }
        // public static void RaiseDrawCardRequested(DeckType t)
        // {
        //     DrawCardRequested?.Invoke(t);
        // }
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
    // public struct CardDrawn 
    // {
    //     public DeckType deck; 
    //     public string title; 
    //     public string body;
    // }
}
