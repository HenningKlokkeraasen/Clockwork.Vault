namespace Clockwork.Vault.Integrations.Tidal.Dao.Models
{
    public class TidalUserFavoritePlaylist : TidalUserFavoriteBase
    {
        public string PlaylistId { get; set; }
        public TidalPlaylist Playlist { get; set; }
    }
}