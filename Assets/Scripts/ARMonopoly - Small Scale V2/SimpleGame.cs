// Assets/Scripts/Simple/SimpleGame.cs
using System.Collections.Generic;
using UnityEngine;

namespace ARMonopoly.Simple
{
    public static class SimpleGame
    {
        // propertyId -> ownerId (-1 if unowned)
        private static readonly Dictionary<string, int> owner = new();
        // playerId -> money (mirror what's in PlayerTag for UI consistency)
        private static readonly Dictionary<int, int> money = new();

        public static void RegisterPlayer(PlayerTag p)
        {
            if (!money.ContainsKey(p.playerId)) money[p.playerId] = p.money;
        }

        public static int GetOwner(string propId)
        {
            return owner.TryGetValue(propId, out var o) ? o : -1;
        }

        public static int GetMoney(int playerId) => money.TryGetValue(playerId, out var m) ? m : 0;

        public static bool CanBuy(PlayerTag p, PropertyTag prop) =>
            GetOwner(prop.propertyId) == -1 && GetMoney(p.playerId) >= prop.price;

        public static bool Buy(PlayerTag p, PropertyTag prop)
        {
            if (!CanBuy(p, prop)) return false;
            money[p.playerId] -= prop.price;
            p.money = money[p.playerId];
            owner[prop.propertyId] = p.playerId;
            SimpleUI.Log($"{p.playerName} bought {prop.displayName} for ${prop.price}.");
            SimpleUI.RefreshMoney(p.playerId, money[p.playerId]);
            return true;
        }

        public static void PayRent(PlayerTag payer, PropertyTag prop)
        {
            int o = GetOwner(prop.propertyId);
            if (o == -1 || o == payer.playerId) return;

            int rent = Mathf.Max(1, prop.baseRent);
            money[payer.playerId] -= rent;
            if (!money.ContainsKey(o)) money[o] = 0;
            money[o] += rent;

            payer.money = money[payer.playerId];
            SimpleUI.Log($"{payer.playerName} paid ${rent} rent to P{o} for {prop.displayName}.");
            SimpleUI.RefreshMoney(payer.playerId, money[payer.playerId]);
            SimpleUI.RefreshMoney(o, money[o]);
        }
    }
}
