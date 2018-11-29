namespace Clockwork.Vault.Dao.Models.Tidal
{
    public class TidalTrack : TidalMusicalWorkBase
    {
        public int TrackNumber { get; set; }
        public int VolumeNumber { get; set; }
        public string Isrc { get; set; }
    }
}