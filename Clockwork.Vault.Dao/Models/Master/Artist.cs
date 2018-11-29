using Clockwork.Vault.Dao.Models.Core;

namespace Clockwork.Vault.Dao.Models.Master
{
    public class Artist : AppGeneratedEntityBase
    {
        public string Name { get; set; }

        public SourceEnum Source { get; set; }
    }
}