using ARMonopoly_V4___Full_Scale_V1._Previous_Scripts_.Core;
using ARMonopoly_V4___Full_Scale_V1._Previous_Scripts_.Data;
using ARMonopoly_V4___Full_Scale_V1._Previous_Scripts_.Services;
using UnityEngine;

namespace ARMonopoly_V4___Full_Scale_V1._Previous_Scripts_.Cards
{
    public class CardResolver : MonoBehaviour
    {
        public CardDeck chanceDeck;
        public CardDeck communityDeck;

        private System.Random rng;

        private void Awake()
        {
            rng = new System.Random();
        }

        private void OnEnable()
        {
            GameEvents.DrawCardRequested += OnDraw;
        }

        private void OnDisable()
        {
            GameEvents.DrawCardRequested -= OnDraw;
        }

        private void OnDraw(DeckType t)
        {
            var deck = t == DeckType.Chance ? chanceDeck : communityDeck;
            if (deck == null || deck.cards == null || deck.cards.Length == 0) 
                return;

            var card = deck.cards[rng.Next(deck.cards.Length)];
            GameEvents.RaiseCardDrawn(new CardDrawn
            {
                deck=t, 
                title=card.title, 
                body=card.body
            });

            // Apply effect (minimal set)
            switch (card.effectType)
            {
                case CardEffectType.ReceiveFromBank:
                    FindService<EconomyService>().Credit(CurrentPlayerId.Value, card.intParam);
                    break;
                case CardEffectType.PayBank:
                    FindService<EconomyService>().Debit(CurrentPlayerId.Value, card.intParam);
                    break;
                case CardEffectType.GoToJail:
                    GameEvents.RaiseSentToJail(CurrentPlayerId.Value);
                    break;
                case CardEffectType.GetOutOfJail:
                    // you'd flip a flag on the player; omitted for brevity
                    break;
                // AdvanceToId / AdvanceRelative would be board-index based; since this is all-ImageTarget,
                // you can simulate by logging or instructing the user token movement manually.
            }
        }

        // Extremely simple "service locator" for this small project:
        private T FindService<T>() where T : class => FindObjectOfType<AppGame>()?.GetService<T>();
        // Current active player id, set by TurnController via AppGame (see below)
        private AppGame App => FindObjectOfType<AppGame>();
        private int? CurrentPlayerId => App ? App.CurrentPlayerId : null;
    }
}
