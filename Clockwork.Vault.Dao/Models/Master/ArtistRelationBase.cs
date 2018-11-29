using Clockwork.Vault.Dao.Models.Core;

namespace Clockwork.Vault.Dao.Models.Master
{
    public abstract class ArtistRelationBase : AppGeneratedEntityBase
    {
        public int ArtistId { get; set; }
        public virtual Artist Artist { get; set; }

        public string Type { get; set; }
    }
}