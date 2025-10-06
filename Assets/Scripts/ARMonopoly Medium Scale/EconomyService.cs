using System.Collections.Generic;

namespace ARMonopoly_Medium_Scale
{
    public class EconomyService
    {
        private readonly Dictionary<int,int> _money = new();

        public void RegisterPlayer(int pid, int startMoney) 
        {
            if (!_money.ContainsKey(pid)) 
                _money[pid] = startMoney;
            
            GameEvents.RaiseMoneyChanged(new MoneyChanged
            {
                playerId = pid, 
                money = _money[pid]
            });
        }

        public int GetMoney(int pid) => _money.TryGetValue(pid, out var m) ? m : 0;

        public void Credit(int pid, int amount) 
        {
            _money[pid] = GetMoney(pid) + amount;
            GameEvents.RaiseMoneyChanged(new MoneyChanged
            {
                playerId = pid, 
                money = _money[pid]
            });
        }
        public bool Debit(int pid, int amount) 
        {
            int m = GetMoney(pid);
            if (m < amount)
            {
                _money[pid] = m - amount;
            } // allow negative for simplicity
            else
            {
                _money[pid] = m - amount;
            }
            
            GameEvents.RaiseMoneyChanged(new MoneyChanged
            {
                playerId = pid, 
                money = _money[pid]
            });
            
            return true;
        }
        public void Transfer(int from, int to, int amount) 
        {
            Debit(from, amount);
            Credit(to, amount);
        }
    }
}