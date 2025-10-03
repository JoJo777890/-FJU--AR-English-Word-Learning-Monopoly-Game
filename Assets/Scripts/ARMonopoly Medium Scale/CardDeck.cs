using UnityEngine;

namespace ARMonopoly_Medium_Scale
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