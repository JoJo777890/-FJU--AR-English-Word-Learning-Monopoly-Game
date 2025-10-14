using System.Collections.Generic;

namespace ARMonopoly_V4___Full_Scale_V1._Previous_Scripts_.Services
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