namespace Clockwork.Vault.Integrations.Tidal.Dao.Models
{
    public class TidalUserFavoritePlaylist : TidalUserFavoriteBase
    {
        public string PlaylistId { get; set; }
        public virtual TidalPlaylist Playlist { get; set; }
    }
}