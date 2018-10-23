using System.Collections.Generic;
using Clockwork.Vault.Integrations.Tidal.Dao.Models;

namespace Clockwork.Vault.Integrations.Tidal.Orchestration.ViewModels
{
    public class TidalPlaylistExpanded
    {
        public TidalPlaylist Playlist { get; set; }
        public ICollection<TidalTrackExpanded> Tracks { get; set; }
    }
}