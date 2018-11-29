using Clockwork.Vault.Dao.Models.Core;

namespace Clockwork.Vault.Dao.Models.Master
{
    public abstract class MusicalWorkBase : AppGeneratedEntityBase
    {
        public string Title { get; set; }
        public string Version { get; set; }
        public int Duration { get; set; }

        public SourceEnum Source { get; set; }
        public bool PossiblyDuplicate { get; set; }
    }
}