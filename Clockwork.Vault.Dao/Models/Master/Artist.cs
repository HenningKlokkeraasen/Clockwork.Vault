using Clockwork.Vault.Dao.Models.Core;

namespace Clockwork.Vault.Dao.Models.Master
{
    public class Artist : AppGeneratedEntityBase
    {
        public string Name { get; set; }

        public SourceEnum Source { get; set; }

        // The ID the entity has in the source
        public int SourceId { get; set; }
    }
}