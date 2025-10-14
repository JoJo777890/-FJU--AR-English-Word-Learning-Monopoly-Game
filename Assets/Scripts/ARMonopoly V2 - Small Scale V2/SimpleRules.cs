// Assets/Scripts/Simple/SimpleRules.cs
using UnityEngine;

namespace ARMonopoly.Simple
{
    public class SimpleRules : MonoBehaviour
    {
        private void OnEnable()
        {
            GameEvents.PropertyLanded += OnPropertyLanded;
            GameEvents.BuyRequested    += OnBuyRequested;
        }
        private void OnDisable()
        {
            GameEvents.PropertyLanded -= OnPropertyLanded;
            GameEvents.BuyRequested    -= OnBuyRequested;
        }

        private void OnPropertyLanded(PropertyLanded e)
        {
            var tag = FindPropertyById(e.propertyId);
            if (tag == null) return;

            int ownerId = SimpleGame.GetOwner(e.propertyId);
            if (ownerId == -1)
            {
                // unowned -> prompt UI
                GameEvents.RaiseBuyPrompt(new BuyPrompt{
                    playerId = e.playerId, 
                    propertyId = e.propertyId, 
                    propertyName = e.propertyName, 
                    price = tag.price
                });
            }
            else
            {
                // owned -> pay rent
                SimpleGame.PayRent(e.playerId, ownerId, tag);
            }
        }

        private void OnBuyRequested(BuyRequest req)
        {
            var tag = FindPropertyById(req.propertyId);
            if (tag == null) return;
            SimpleGame.Buy(req.playerId, tag);
        }

        private PropertyTag FindPropertyById(string id)
        {
            foreach (var p in GameObject.FindObjectsOfType<PropertyTag>())
                if (p.propertyId == id) return p;
            return null;
        }
    }
}