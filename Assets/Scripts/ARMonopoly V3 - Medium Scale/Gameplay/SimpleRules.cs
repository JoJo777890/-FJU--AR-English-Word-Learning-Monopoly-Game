// SimpleRules.cs

using ARMonopoly_V3___Medium_Scale.Board;
using ARMonopoly_V3___Medium_Scale.Core;
using ARMonopoly_V3___Medium_Scale.Services;
using UnityEngine;

namespace ARMonopoly_V3___Medium_Scale.Gameplay
{
    public class SimpleRules : MonoBehaviour
    {
        private AppGame _app;
        private EconomyService _eco;
        private OwnershipService _own;

        private void OnEnable()
        {
            GameEvents.PropertyLanded += OnPropertyLanded;
            GameEvents.BuyRequested   += OnBuyRequested;
            EnsureServices();
        }
        private void OnDisable()
        {
            GameEvents.PropertyLanded -= OnPropertyLanded;
            GameEvents.BuyRequested   -= OnBuyRequested;
        }

        private void EnsureServices()
        {
            if (_app == null)
            {
                _app = FindObjectOfType<AppGame>();
            }
            
            if (_app != null)
            {
                _eco = _app.GetService<EconomyService>();
                _own = _app.GetService<OwnershipService>();
            }
        }

        private bool ServicesReady()
        {
            EnsureServices();
            if (_app == null || _eco == null || _own == null)
            {
                Debug.LogError($"[SimpleRules] Services not ready. App={_app!=null}, Eco={_eco!=null}, Own={_own!=null}");
                return false;
            }
            return true;
        }

        private void OnPropertyLanded(PropertyLanded e)
        {
            if (!ServicesReady()) return;

            if (string.IsNullOrEmpty(e.propertyId))
            {
                Debug.LogError("[SimpleRules] propertyId null/empty.");
                return;
            }

            var tag = FindPropertyById(e.propertyId);
            if (tag == null)
            {
                Debug.LogError($"[SimpleRules] PropertyTag not found for id='{e.propertyId}'.");
                return;
            }

            int owner = _own.GetOwner(tag.Id);
            if (owner == -1)
            {
                GameEvents.RaiseBuyPrompt(new BuyPrompt
                {
                    playerId = e.playerId, 
                    propertyId = tag.Id, 
                    propertyName = tag.DisplayName, 
                    price = tag.Price
                });
            }
            else if (owner != e.playerId)
            {
                int rent = Mathf.Max(1, tag.BaseRent);
                _eco.Transfer(e.playerId, owner, rent);
                GameEvents.RaiseRentPaid(new RentPaid
                {
                    payerId=e.playerId, 
                    ownerId=owner, 
                    propertyId=tag.Id, 
                    propertyName=tag.DisplayName, 
                    amount=rent
                });
            }
        }

        private void OnBuyRequested(BuyRequest req)
        {
            if (!ServicesReady()) 
                return;

            var tag = FindPropertyById(req.propertyId);
            
            if (tag == null)
            {
                Debug.LogError($"[SimpleRules] BuyRequested: missing tag '{req.propertyId}'."); 
                return;
            }
            if (_own.GetOwner(tag.Id) != -1) 
                return;

            int price = tag.Price;
            if (_eco.GetMoney(req.playerId) >= price)
            {
                _eco.Debit(
                    req.playerId, 
                    price
                    );
                _own.SetOwner(
                    tag.Id, 
                    req.playerId
                    );
                
                GameEvents.RaisePropertyBought(new PropertyBought
                {
                    playerId=req.playerId, 
                    propertyId=tag.Id, 
                    propertyName=tag.DisplayName, 
                    price=price
                });
            }
        }

        private PropertyTag FindPropertyById(string id)
        {
            foreach (var t in FindObjectsOfType<PropertyTag>())
                if (t.Id == id) 
                    return t;
            return null;
        }
    }
}
