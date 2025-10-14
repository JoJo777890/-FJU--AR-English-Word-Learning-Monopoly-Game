using ARMonopoly_V4___Full_Scale_V1._Previous_Scripts_.Data;
using UnityEngine;

namespace ARMonopoly_V4___Full_Scale_V1._Previous_Scripts_.Board 
{
    public class PropertyTag : MonoBehaviour {
        public PropertyDef def;
        public string Id => def ? def.id : gameObject.name;
        public string DisplayName => def ? def.displayName : name;
        public int Price => def ? def.price : 0;
        public int BaseRent => (def && def.rentTiers.Length>0) ? def.rentTiers[0] : 1;
    }
}
