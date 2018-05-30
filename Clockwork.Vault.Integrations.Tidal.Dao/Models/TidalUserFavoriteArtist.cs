namespace Clockwork.Vault.Integrations.Tidal.Dao.Models
{
    public class TidalUserFavoriteArtist : TidalUserFavoriteBase
    {
        public int ArtistId { get; set; }
        public TidalArtist Artist { get; set; }
    }
}