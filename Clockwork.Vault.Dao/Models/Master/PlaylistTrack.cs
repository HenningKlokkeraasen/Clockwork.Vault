namespace Clockwork.Vault.Dao.Models.Master
{
    public class PlaylistTrack : TrackRelationBase
    {
        public string PlaylistId { get; set; }
        public virtual Playlist Playlist { get; set; }
    }
}