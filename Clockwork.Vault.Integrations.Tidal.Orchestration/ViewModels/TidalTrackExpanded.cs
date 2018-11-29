using System.Collections.Generic;
using Clockwork.Vault.Dao.Models.Tidal;

namespace Clockwork.Vault.Integrations.Tidal.Orchestration.ViewModels
{
    public class TidalTrackExpanded
    {
        public TidalTrack Track { get; set; }
        public ICollection<TidalArtist> Artists { get; set; }
    }
}