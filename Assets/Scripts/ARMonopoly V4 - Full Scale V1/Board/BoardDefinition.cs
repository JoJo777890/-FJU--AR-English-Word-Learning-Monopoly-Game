using System;
using UnityEngine;

namespace ARMonopoly_V4___Full_Scale_V1.Board
{
    [CreateAssetMenu(menuName = "ARMonopolyV4-FullScale/BoardDefinition")]
    public class BoardDefinition : ScriptableObject
    {
        public int goIndex = 0;
        public int jailIndex = 10;
        public BoardSpace[] spaces; // 0..N-1 clockwise
    }
    
    [Serializable]
    public struct BoardSpace
    {
        public string id;            // unique, e.g., "TOKYO_TOWER"
        public string displayName;   // localized key ok
        public SpaceType type;
        public string propertyId;    // filled if SpaceType.Property
    }
    
    public enum SpaceType 
    {
        Start, 
        Property
    }
}


