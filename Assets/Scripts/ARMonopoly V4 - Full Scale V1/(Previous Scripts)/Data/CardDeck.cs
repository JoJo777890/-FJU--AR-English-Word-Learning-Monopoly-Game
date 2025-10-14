using UnityEngine;

namespace ARMonopoly_V4___Full_Scale_V1._Previous_Scripts_.Data
{
    public enum DeckType 
    {
        Chance, 
        Community
    }
    
    public enum CardEffectType {
        AdvanceToId, 
        GoToJail, 
        GetOutOfJail, 
        ReceiveFromBank, 
        PayBank, 
        AdvanceRelative, 
        Repairs
    }

    [System.Serializable]
    public class CardItem
    {
        public string id;
        public string title;
        [TextArea] public string body;
        public CardEffectType effectType;
        public string strParam; // e.g., propertyId for AdvanceToId
        public int intParam;    // e.g., amount or steps
    }

    [CreateAssetMenu(menuName="ARMonopolyV2-MediumScale/Card Deck")]
    public class CardDeck : ScriptableObject
    {
        public DeckType type;
        public CardItem[] cards;
    }
}