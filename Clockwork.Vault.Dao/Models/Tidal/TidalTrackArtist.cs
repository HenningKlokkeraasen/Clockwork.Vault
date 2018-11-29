namespace Clockwork.Vault.Dao.Models.Tidal
{
    public class TidalTrackArtist : TidalArtistRelationBase
    {
        public int TrackId { get; set; }
        public virtual TidalTrack Track { get; set; }
    }
}