using System;
using UnityEngine;

namespace ARMonopoly.Data
{
    [CreateAssetMenu(fileName = "BoardDefinition", menuName = "ARMonopoly/BoardDefinition")]
    public class BoardDefinition : ScriptableObject
    {
        public int goIndex = 0;
        public int jailIndex = 10;
        public BoardSpace[] space;
    }

    [Serializable]
    public struct BoardSpace
    {
        public string id; // e.g., "Taipei_101"
        public string displayName;
        public SpaceType type;
        public string propertyId;
    }

    public enum SpaceType
    {
        Start, 
        Property, 
        Chance, 
        CommunityChest, 
        Tax, 
        Jail, 
        GoToJail, 
        FreeParking
    }
}

