namespace Clockwork.Vault.Dao.Models.Master
{
    public class FavoriteAlbum : FavoriteBase
    {
        public int AlbumId { get; set; }
        public virtual Album Album { get; set; }
    }
}