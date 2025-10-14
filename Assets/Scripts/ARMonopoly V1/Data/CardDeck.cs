using System;
using UnityEngine;

namespace ARMonopoly.Data
{
    [CreateAssetMenu(fileName = "CardDeck", menuName = "ARMonopoly/CardDeck")]
    public class CardDeck : ScriptableObject
    {
        public DeckType deckType;
        public CardItem[] cards;
    }

    public enum DeckType
    {
        Chance, 
        CommunityChest
    }

    [Serializable]
    public struct CardItem
    {
        public string id;
        public string title;
        [TextArea] public string body;
        public CardEffectType effectType;
        public int intParam; // e.g., steps, payment, ...
        public string strParam; // e.g., "GoToGoArea", "PayPerHouse", ...
        
        // e.g., Advance to Go:     intParam=-1 , strParam="GoToGoArea"
        // e.g., Pay $40 per house: intParam=-40, strParam="PayPerHouse"
    }

    public enum CardEffectType
    {
        AdvanceTo, 
        GetOutOfJail, 
        GoToJail, 
        PayBank, 
        ReceiveFromBank
        // and anything else...
    }
}


