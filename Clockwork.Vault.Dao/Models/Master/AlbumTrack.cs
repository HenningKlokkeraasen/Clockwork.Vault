namespace Clockwork.Vault.Dao.Models.Master
{
    public class AlbumTrack : TrackRelationBase
    {
        public int AlbumId { get; set; }
        public virtual Album Album { get; set; }
    }
}