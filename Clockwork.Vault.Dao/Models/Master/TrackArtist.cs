namespace Clockwork.Vault.Dao.Models.Master
{
    public class TrackArtist : ArtistRelationBase
    {
        public int TrackId { get; set; }
        public virtual Track Track { get; set; }
    }
}