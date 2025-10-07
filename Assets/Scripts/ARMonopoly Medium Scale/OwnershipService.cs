using System.Collections.Generic;

namespace ARMonopoly_Medium_Scale
{
    public class OwnershipService
    {
        private readonly Dictionary<string,int> _ownerByProp = new(); // propId -> ownerId

        public int GetOwner(string propId)
        {
            return _ownerByProp.TryGetValue(propId, out var o) ? o : -1;
        }
        public void SetOwner(string propId, int ownerId)
        {
            _ownerByProp[propId] = ownerId;
        }
    }
}