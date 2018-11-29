namespace Clockwork.Vault.Dao.Models.Master
{
    public class FavoriteTrack : FavoriteBase
    {
        public int TrackId { get; set; }
        public virtual Track Track { get; set; }
    }
}