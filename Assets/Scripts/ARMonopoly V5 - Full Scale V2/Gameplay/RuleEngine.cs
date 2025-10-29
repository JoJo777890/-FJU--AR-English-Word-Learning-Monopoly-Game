using ARMonopoly_V5___Full_Scale_V2.Core;
using ARMonopoly_V5___Full_Scale_V2.Data;
using ARMonopoly_V5___Full_Scale_V2.Economy;
using UnityEngine;

namespace ARMonopoly_V5___Full_Scale_V2.Gameplay
{
    /// <summary>
    /// Listens for game events (like landing) and applies rules.
    /// This is the core "logic" of the game.
    /// </summary>
    public class RuleEngine : MonoBehaviour
    {
        private AppGame _app;
        private Bank _bank;
        private RentCalculator _rentCalc;
        private TurnController _turnController;

        // State to track if a turn's landing has been resolved
        private bool _turnResolved = false;

        void Start()
        {
            _app = AppGame.Instance;
            _bank = _app.Bank;
            _rentCalc = _app.RentCalculator;
            _turnController = FindObjectOfType<TurnController>();
        }

        void OnEnable()
        {
            GameEvents.OnProximityEnter += HandleProximityEnter;
            GameEvents.OnBuyRequested += HandleBuyRequest;
            GameEvents.OnTurnStarted += OnTurnStarted;
        }

        void OnDisable()
        {
            GameEvents.OnProximityEnter -= HandleProximityEnter;
            GameEvents.OnBuyRequested -= HandleBuyRequest;
            GameEvents.OnTurnStarted -= OnTurnStarted;
        }

        void OnTurnStarted(int playerID)
        {
            // Reset the resolution flag for the new turn
            _turnResolved = false;
        }

        void HandleProximityEnter(ProximityPayload payload)
        {
            // Only process the *first* landing event of a turn
            if (_turnResolved) return;
            
            // Only process if it's the current player
            if (payload.PlayerID != _turnController.GetCurrentPlayerID()) return;

            // Mark turn as resolved so we don't process another landing
            _turnResolved = true;
            _app.StateMachine.SetState(GameState.ResolvingTurn);

            var property = payload.PropertyDef;
            if (property == null)
            {
                AdvanceTurn(); // Landed on invalid space
                return;
            }

            int ownerID = _app.GetPropertyOwner(property.PropertyID);
            
            if (ownerID == -1) // Unowned
            {
                // Check if player can afford it
                if (_app.GetWallet(payload.PlayerID).GetBalance() >= property.Price)
                {
                    GameEvents.RaiseBuyPrompt(new BuyPromptPayload
                    {
                        PlayerID = payload.PlayerID,
                        PropertyID = property.PropertyID,
                        DisplayName = property.DisplayName,
                        Price = property.Price
                    });
                    // The UI will either call HandleBuyRequest or the player might "pass"
                    // We need a "Pass" button in the UI that calls AdvanceTurn()
                }
                else
                {
                    GameEvents.RaiseNotify($"Player {payload.PlayerID} cannot afford {property.DisplayName}.");
                    AdvanceTurn();
                }
            }
            else if (ownerID == payload.PlayerID) // Landed on own property
            {
                GameEvents.RaiseNotify($"Player {payload.PlayerID} landed on their own property: {property.DisplayName}.");
                AdvanceTurn();
            }
            else // Landed on someone else's property
            {
                HandleRent(payload.PlayerID, ownerID, property);
                AdvanceTurn();
            }
        }

        void HandleBuyRequest(BuyRequestPayload payload)
        {
            // Ensure the state is correct (i.e., we are resolving a turn)
            if (_app.StateMachine.CurrentState != GameState.ResolvingTurn) return;

            var property = _app.PropertyDB.GetProperty(payload.PropertyID);
            if (property == null) return;

            Wallet buyerWallet = _app.GetWallet(payload.PlayerID);
            
            if (_bank.Pay(buyerWallet, property.Price))
            {
                _app.SetPropertyOwner(property.PropertyID, payload.PlayerID);
                GameEvents.RaisePropertyBought(new PropertyBoughtPayload
                {
                    PlayerID = payload.PlayerID,
                    PropertyID = property.PropertyID,
                    Price = property.Price
                });
                GameEvents.RaiseNotify($"Player {payload.PlayerID} bought {property.DisplayName}!");
            }
            
            // Whether they bought it or not, the turn is resolved.
            AdvanceTurn();
        }

        void HandleRent(int payerID, int ownerID, PropertyDef property)
        {
            int rent = _rentCalc.CalculateRent(property);
            Wallet payerWallet = _app.GetWallet(payerID);
            Wallet ownerWallet = _app.GetWallet(ownerID);

            if (_bank.Transfer(payerWallet, ownerWallet, rent))
            {
                GameEvents.RaiseNotify($"Player {payerID} paid ${rent} rent to Player {ownerID} for {property.DisplayName}.");
            }
            else
            {
                GameEvents.RaiseNotify($"Player {payerID} cannot afford ${rent} rent for {property.DisplayName}!");
                // Here you would implement bankruptcy logic
            }
        }
        
        // Call this when a turn's action is complete
        public void AdvanceTurn()
        {
            // TODO: Check for doubles rule. If doubles, don't advance.
            // For now, we always advance.
            _turnController.AdvanceToNextPlayer();
        }
    }
}
