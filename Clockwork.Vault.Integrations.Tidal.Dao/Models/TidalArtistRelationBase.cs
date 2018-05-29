namespace Clockwork.Vault.Integrations.Tidal.Dao.Models
{
    public abstract class TidalArtistRelationBase : AppGeneratedEntityBase
    {
        public int ArtistId { get; set; }
        public virtual TidalArtist Artist { get; set; }

        public string Type { get; set; }
    }
}