using System.Collections.Generic;
using Clockwork.Vault.Integrations.Tidal.Dao.Models;

namespace Clockwork.Vault.Integrations.Tidal.Orchestration.ViewModels
{
    public class TidalAlbumExpanded
    {
        public TidalAlbum Album { get; set; }
        public ICollection<TidalArtist> Artists { get; set; }
        public ICollection<TidalTrackExpanded> Tracks { get; set; }
    }
}