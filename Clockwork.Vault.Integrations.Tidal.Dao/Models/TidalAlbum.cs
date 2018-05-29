using System;

namespace Clockwork.Vault.Integrations.Tidal.Dao.Models
{
    public class TidalAlbum : TidalMusicalWorkBase
    {
        public int NumberOfTracks { get; set; }
        public int NumberOfVolumes { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Type { get; set; }
        public string Upc { get; set; }
        public string Cover { get; set; }
    }
}