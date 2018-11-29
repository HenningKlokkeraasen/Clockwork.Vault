namespace Clockwork.Vault.Dao.Models.Tidal
{
    public class TidalUserFavoriteArtist : TidalUserFavoriteBase
    {
        public int ArtistId { get; set; }
        public virtual TidalArtist Artist { get; set; }
    }
}