namespace Clockwork.Vault.Dao.Models.Tidal
{
    public class TidalUserFavoriteAlbum : TidalUserFavoriteBase
    {
        public int AlbumId { get; set; }
        public virtual TidalAlbum Album { get; set; }
    }
}