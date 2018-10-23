namespace Clockwork.Vault.Integrations.Tidal.Dao.Models
{
    public abstract class TidalTrackRelationBase : AppGeneratedEntityBase
    {
        public int TrackId { get; set; }
        public virtual TidalTrack Track { get; set; }

        public int Position { get; set; }
    }
}