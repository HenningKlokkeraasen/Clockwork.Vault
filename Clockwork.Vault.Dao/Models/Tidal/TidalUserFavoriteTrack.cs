namespace Clockwork.Vault.Dao.Models.Tidal
{
    public class TidalUserFavoriteTrack : TidalUserFavoriteBase
    {
        public int TrackId { get; set; }
        public virtual TidalTrack Track { get; set; }
    }
}