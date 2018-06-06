using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clockwork.Vault.Integrations.Tidal.Dao.Models
{
    public class TidalPlaylist
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Uuid { get; set; }

        public string Type { get; set; }
        public bool PublicPlaylist { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? LastUpdated { get; set; }
        public int NumberOfTracks { get; set; }
        public int Duration { get; set; }

        public int CreatorId { get; set; }
        public virtual TidalCreator Creator { get; set; }
    }
}
