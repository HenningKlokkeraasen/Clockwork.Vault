namespace Clockwork.Vault.Dao.Models.Master
{
    public class Track : MusicalWorkBase
    {
        public int TrackNumber { get; set; }
        public int VolumeNumber { get; set; }
        public string Isrc { get; set; }

        public SourceEnum Source { get; set; }
    }
}