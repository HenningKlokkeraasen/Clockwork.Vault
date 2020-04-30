namespace Clockwork.Vault.Dao.Models.Master
{
    public class AlbumArtist : ArtistRelationBase
    {
        public int AlbumId { get; set; }

        public SourceEnum Source { get; set; }

        // The ID the entity has in the source
        public string SourceId { get; set; }
    }
}