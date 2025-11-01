using ARMonopoly_V5___Full_Scale_V2.Board;
using ARMonopoly_V5___Full_Scale_V2.Core;
using ARMonopoly_V5___Full_Scale_V2.Data;
using ARMonopoly_V5___Full_Scale_V2.Economy;
using UnityEngine;

namespace ARMonopoly_V5___Full_Scale_V2.Gameplay
{
    /// <summary>
    /// Listens for game events (like landing) and applies game rules.
    /// Attached to the [GameSystems] GameObject.
    /// </summary>
    public class RuleEngine : MonoBehaviour
    {
        private Bank _bank;
        private BoardDefinition _board;
        private RentCalculator _rentCalculator;
        private GameStateMachine _stateMachine;
        private TurnController _turnController;

        // Use Awake for safe reference gathering
        private void Awake()
        {
            _bank = AppGame.Instance.Bank;
            _board = AppGame.Instance.Board;
            _rentCalculator = AppGame.Instance.RentCalculator;
            _stateMachine = AppGame.Instance.StateMachine;
            
            // This component depends on the TurnController
            _turnController = GetComponent<TurnController>();

            if (_bank == null || _board == null || _rentCalculator == null || _stateMachine == null || _turnController == null)
            {
                Debug.LogError("RuleEngine: Missing one or more critical references from AppGame or GameObject!");
            }
        }

        // Use OnEnable/OnDisable for event subscriptions
        private void OnEnable()
        {
            GameEvents.OnProximityEnter += HandleProximityEnter;
            GameEvents.OnBuyRequest += HandleBuyRequest;
            GameEvents.OnPlayerPassedGo += HandlePassedGo;
        }

        private void OnDisable()
        {
            GameEvents.OnProximityEnter -= HandleProximityEnter;
            GameEvents.OnBuyRequest -= HandleBuyRequest;
            GameEvents.OnPlayerPassedGo -= HandlePassedGo;
        }

        private void HandlePassedGo(int playerID)
        {
            int passGoMoney = AppGame.Instance.Config.PassGoMoney;
            Wallet wallet = _bank.GetWallet(playerID);
            if (wallet != null)
            {
                wallet.Add(passGoMoney);
                Debug.Log($"Player {playerID} passed Go, credited ${passGoMoney}");
            }
            else
            {
                 Debug.LogWarning($"RuleEngine: Could not find wallet for Player {playerID} to credit Pass Go money.");
            }
        }

        private void HandleProximityEnter(ProximityPayload payload)
        {
            // 1. Is the game waiting for this move?
            if (_stateMachine.CurrentState != GameState.AwaitingPlayerMove) return;

            // 2. Is it the correct player?
            if (payload.PlayerID != _turnController.CurrentPlayerID) return;

            // 3. Is it the correct destination?
            if (payload.PropertyID != AppGame.Instance.ExpectedDestinationPropertyID)
            {
                Debug.Log($"Player {payload.PlayerID} landed on {payload.PropertyID}, but we are waiting for {AppGame.Instance.ExpectedDestinationPropertyID}");
                return;
            }

            // --- SUCCESS ---
            Debug.Log($"Player {payload.PlayerID} correctly landed on {payload.PropertyID}. Resolving space.");
            
            // Change state to show we are resolving
            _stateMachine.SetState(GameState.ResolvingSpace);
            
            // Get the landed property's data
            PropertyDef landedProp = _board.GetPropertyAt(_board.GetIndexFromID(payload.PropertyID));
            if (landedProp == null)
            {
                Debug.LogError($"Could not find PropertyDef for ID {payload.PropertyID}");
                _turnController.EndTurn(); // Failsafe
                return;
            }

            // 4. Resolve the landing (Buy / Rent / etc.)
            int ownerID = _bank.GetPropertyOwner(landedProp.PropertyID);

            if (ownerID == -1)
            {
                // Unowned. Prompt to buy.
                GameEvents.RaiseBuyPrompt(new BuyPayload
                {
                    PlayerID = payload.PlayerID,
                    PropertyID = landedProp.PropertyID,
                    PropertyName = landedProp.DisplayName,
                    Price = landedProp.Price // <-- FIXED: Was PricePrice
                });
                // The UIManager will now show the Buy Panel.
                // The turn will end when the player clicks "Buy" or "Pass".
            }
            else if (ownerID == payload.PlayerID)
            {
                // Landed on their own property. Do nothing.
                Debug.Log($"Player {payload.PlayerID} landed on their own property.");
                _turnController.EndTurn();
            }
            else
            {
                // Landed on someone else's property. Pay rent.
                int rentAmount = _rentCalculator.CalculateRent(landedProp);
                bool success = _bank.TransferRent(payload.PlayerID, ownerID, rentAmount);

                if (success)
                {
                    GameEvents.RaiseRentPaid(new RentPayload
                    {
                        PayerID = payload.PlayerID,
                        OwnerID = ownerID,
                        PropertyID = landedProp.PropertyID,
                        PropertyName = landedProp.DisplayName,
                        Amount = rentAmount
                    });
                }
                else
                {
                    Debug.LogWarning($"Player {payload.PlayerID} could not afford rent!");
                    // (Future) Implement bankruptcy logic here
                }
                _turnController.EndTurn();
            }
        }

        private void HandleBuyRequest(string propertyID)
        {
            // Only process buy requests if we are in the ResolvingSpace state
            if (_stateMachine.CurrentState != GameState.ResolvingSpace) return;
            
            int playerID = _turnController.CurrentPlayerID;
            PropertyDef propToBuy = AppGame.Instance.PropertyDB.GetProperty(propertyID);

            if (propToBuy == null)
            {
                Debug.LogError($"RuleEngine: Could not find PropertyDef for {propertyID} to buy.");
                _turnController.EndTurn();
                return;
            }
            
            bool success = _bank.BuyProperty(playerID, propToBuy);

            if (success)
            {
                GameEvents.RaisePropertyBought(new PropertyPayload
                {
                    PlayerID = playerID,
                    PropertyID = propToBuy.PropertyID,
                    PropertyName = propToBuy.DisplayName,
                    Price = propToBuy.Price
                });
            }
            else
            {
                Debug.Log($"Player {playerID} failed to buy {propToBuy.DisplayName}. (Not enough money or already owned)");
            }

            // Whether buy succeeded or failed, the action is resolved. End the turn.
            _turnController.EndTurn();
        }
    }
}

