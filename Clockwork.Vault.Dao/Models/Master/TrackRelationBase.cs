using Clockwork.Vault.Dao.Models.Core;

namespace Clockwork.Vault.Dao.Models.Master
{
    public abstract class TrackRelationBase : AppGeneratedEntityBase
    {
        public int TrackId { get; set; }
        public virtual Track Track { get; set; }

        public int Position { get; set; }
    }
}