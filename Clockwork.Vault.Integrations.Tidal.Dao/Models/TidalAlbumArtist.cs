namespace Clockwork.Vault.Integrations.Tidal.Dao.Models
{
    public class TidalAlbumArtist : TidalArtistRelationBase
    {
        public int AlbumId { get; set; }
        public virtual TidalAlbum Album { get; set; }
    }
}