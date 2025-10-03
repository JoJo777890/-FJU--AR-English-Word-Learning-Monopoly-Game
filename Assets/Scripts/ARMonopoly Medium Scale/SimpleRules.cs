using UnityEngine;

namespace ARMonopoly_Medium_Scale
{
    public class SimpleRules : MonoBehaviour
    {
        private AppGame _app;
        private EconomyService _eco;
        private OwnershipService _own;

        private void Awake()
        {
            _app = FindObjectOfType<AppGame>();
            _eco = _app.GetService<EconomyService>();
            _own = _app.GetService<OwnershipService>();
        }

        private void OnEnable()
        {
            GameEvents.PropertyLanded += OnPropertyLanded;
            GameEvents.BuyRequested   += OnBuyRequested;
            GameEvents.SentToJail     += OnSentToJail;
        }
        private void OnDisable()
        {
            GameEvents.PropertyLanded -= OnPropertyLanded;
            GameEvents.BuyRequested   -= OnBuyRequested;
            GameEvents.SentToJail     -= OnSentToJail;
        }

        private void OnPropertyLanded(PropertyLanded e)
        {
            var tag = FindTagById(e.propertyId);
            if (!tag) 
                return;

            int owner = _own.GetOwner(tag.Id);
            if (owner == -1)
            {
                GameEvents.RaiseBuyPrompt(new BuyPrompt {
                    playerId = e.playerId, 
                    propertyId = tag.Id, 
                    propertyName = tag.DisplayName, 
                    price = tag.Price
                });
            }
            else if (owner != e.playerId)
            {
                int rent = tag.BaseRent; // tiers/houses can be added later
                _eco.Transfer(e.playerId, owner, rent);
                GameEvents.RaiseRentPaid(new RentPaid{
                    payerId = e.playerId, 
                    ownerId = owner, 
                    propertyId = tag.Id, 
                    propertyName = tag.DisplayName, 
                    amount = rent
                });
            }
        }

        private void OnBuyRequested(BuyRequest req)
        {
            var tag = FindTagById(req.propertyId);
            if (!tag) 
                return;
            int pid = req.playerId;
            if (_own.GetOwner(tag.Id) != -1) 
                return;
            int price = tag.Price;

            if (_eco.GetMoney(pid) >= price)
            {
                _eco.Debit(pid, price);
                _own.SetOwner(tag.Id, pid);
                GameEvents.RaisePropertyBought(new PropertyBought{
                    playerId=pid, 
                    propertyId=tag.Id, 
                    propertyName=tag.DisplayName, 
                    price=price
                });
            }
        }

        private void OnSentToJail(int pid)
        {
            // In an all-ImageTarget game, you can just show UI note "Move your token to Jail area".
        }

        private PropertyTag FindTagById(string id)
        {
            foreach (var t in FindObjectsOfType<PropertyTag>()) 
                if (t.Id == id) 
                    return t;
            return null;
        }
    }
}
