namespace Clockwork.Vault.Integrations.Tidal.Dao.Models
{
    public class TidalTrackArtist : TidalArtistRelationBase
    {
        public int TrackId { get; set; }
        public virtual TidalTrack Track { get; set; }
    }
}