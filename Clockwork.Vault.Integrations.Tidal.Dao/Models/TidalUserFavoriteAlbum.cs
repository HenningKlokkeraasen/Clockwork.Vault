namespace Clockwork.Vault.Integrations.Tidal.Dao.Models
{
    public class TidalUserFavoriteAlbum : TidalUserFavoriteBase
    {
        public int AlbumId { get; set; }
        public virtual TidalAlbum Album { get; set; }
    }
}