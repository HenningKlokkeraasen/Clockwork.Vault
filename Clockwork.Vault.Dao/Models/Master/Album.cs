using System;

namespace Clockwork.Vault.Dao.Models.Master
{
    public class Album : MusicalWorkBase
    {
        public int NumberOfTracks { get; set; }
        public int NumberOfVolumes { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Type { get; set; }
        public string Upc { get; set; }
        public string Cover { get; set; }
    }
}