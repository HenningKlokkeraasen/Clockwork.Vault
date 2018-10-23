namespace Clockwork.Vault.Integrations.Tidal.Dao.Models
{
    public class TidalUserFavoriteTrack : TidalUserFavoriteBase
    {
        public int TrackId { get; set; }
        public virtual TidalTrack Track { get; set; }
    }
}