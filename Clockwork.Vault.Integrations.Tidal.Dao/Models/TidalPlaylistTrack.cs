namespace Clockwork.Vault.Integrations.Tidal.Dao.Models
{
    public class TidalPlaylistTrack : AppGeneratedEntityBase
    {
        public string PlaylistId { get; set; }
        public TidalPlaylist Playlist { get; set; }

        public int TrackId { get; set; }
        public TidalTrack Track { get; set; }
    }
}