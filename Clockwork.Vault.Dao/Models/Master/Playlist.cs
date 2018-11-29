using System;
using Clockwork.Vault.Dao.Models.Core;

namespace Clockwork.Vault.Dao.Models.Master
{   
    public class Playlist : AppGeneratedEntityBase
    {
        public string Type { get; set; }
        public bool IsPublic { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? LastUpdated { get; set; }
        public int NumberOfTracks { get; set; }
        public int Duration { get; set; }
        public int CreatorId { get; set; }
        public virtual Creator Creator { get; set; }
        
        public SourceEnum Source { get; set; }
    }
}
