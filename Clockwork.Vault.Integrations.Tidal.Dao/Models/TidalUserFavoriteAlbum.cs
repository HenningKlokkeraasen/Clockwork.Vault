namespace Clockwork.Vault.Integrations.Tidal.Dao.Models
{
    public class TidalUserFavoriteAlbum : TidalUserFavoriteBase
    {
        public int AlbumId { get; set; }
        public TidalAlbum Album { get; set; }
    }
}