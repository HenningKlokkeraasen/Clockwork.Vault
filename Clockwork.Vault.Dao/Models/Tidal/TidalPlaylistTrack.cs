namespace Clockwork.Vault.Dao.Models.Tidal
{
    public class TidalPlaylistTrack : TidalTrackRelationBase
    {
        public string PlaylistId { get; set; }
        public virtual TidalPlaylist Playlist { get; set; }
    }
}