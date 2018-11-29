namespace Clockwork.Vault.Dao.Models.Master
{
    public class FavoritePlaylist : FavoriteBase
    {
        public string PlaylistId { get; set; }
        public virtual Playlist Playlist { get; set; }
    }
}