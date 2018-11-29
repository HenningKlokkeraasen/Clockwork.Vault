namespace Clockwork.Vault.Dao.Models.Tidal
{
    public class TidalUserFavoritePlaylist : TidalUserFavoriteBase
    {
        public string PlaylistId { get; set; }
        public virtual TidalPlaylist Playlist { get; set; }
    }
}