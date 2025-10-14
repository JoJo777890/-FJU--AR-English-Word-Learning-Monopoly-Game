// Assets/Scripts/Simple/SimpleGame.cs
using System.Collections.Generic;

namespace ARMonopoly.Simple
{
    public static class SimpleGame
    {
        private static readonly Dictionary<string,int> owner = new(); // propertyId -> ownerId (-1 none)
        private static readonly Dictionary<int,int> money = new();    // playerId -> money

        public static void RegisterPlayer(PlayerTag p)
        {
            if (!money.ContainsKey(p.playerId)) money[p.playerId] = p.money;
            GameEvents.RaiseMoneyChanged(new MoneyChanged
            {
                playerId = p.playerId, 
                money = money[p.playerId]
                
            });
        }

        public static int GetMoney(int playerId)
        {
            return money.TryGetValue(playerId, out var m) ? m : 0;
        }
        public static int GetOwner(string propId)
        {
            return owner.TryGetValue(propId, out var o) ? o : -1;
        }

        public static bool Buy(int playerId, PropertyTag prop)
        {
            if (GetOwner(prop.propertyId) != -1) return false;
            int m = GetMoney(playerId);
            if (m < prop.price) return false;

            money[playerId] = m - prop.price;
            owner[prop.propertyId] = playerId;

            GameEvents.RaisePropertyBought(new PropertyBought
            {
                playerId = playerId, 
                propertyId = prop.propertyId, 
                propertyName = prop.displayName, 
                price = prop.price
            });
            GameEvents.RaiseMoneyChanged(new MoneyChanged
            { 
                playerId = playerId, 
                money = money[playerId] 
            });
            return true;
        }

        public static void PayRent(int payerId, int ownerId, PropertyTag prop)
        {
            if (ownerId == -1 || ownerId == payerId) 
                return;
            int rent = prop.baseRent <= 0 ? 1 : prop.baseRent;

            money[payerId]  = GetMoney(payerId)  - rent;
            money[ownerId]  = GetMoney(ownerId) + rent;

            GameEvents.RaiseRentPaid(new RentPaid
            {
                payerId = payerId, 
                ownerId = ownerId, 
                propertyId = prop.propertyId, 
                propertyName = prop.displayName, 
                amount = rent
            });
            GameEvents.RaiseMoneyChanged(new MoneyChanged
            {
                playerId = payerId, 
                money = money[payerId]
            });
            GameEvents.RaiseMoneyChanged(new MoneyChanged
            {
                playerId = ownerId, 
                money = money[ownerId]
            });
        }
    }
}
