namespace Clockwork.Vault.Integrations.Tidal.Dao.Models
{
    public class TidalPlaylistTrack : TidalTrackRelationBase
    {
        public string PlaylistId { get; set; }
        public TidalPlaylist Playlist { get; set; }
    }
}