namespace Clockwork.Vault.Dao.Models.Tidal
{
    public class TidalAlbumTrack : TidalTrackRelationBase
    {
        public int AlbumId { get; set; }
        public virtual TidalAlbum Album { get; set; }
    }
}