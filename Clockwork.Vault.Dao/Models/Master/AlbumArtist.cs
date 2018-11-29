namespace Clockwork.Vault.Dao.Models.Master
{
    public class AlbumArtist : ArtistRelationBase
    {
        public int AlbumId { get; set; }
        public virtual Album Album { get; set; }
    }
}