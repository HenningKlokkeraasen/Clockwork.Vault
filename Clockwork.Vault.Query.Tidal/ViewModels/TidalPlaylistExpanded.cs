using System.Collections.Generic;
using Clockwork.Vault.Dao.Models.Tidal;

namespace Clockwork.Vault.Query.Tidal.ViewModels
{
    public class TidalPlaylistExpanded
    {
        public TidalPlaylist Playlist { get; set; }
        public ICollection<TidalTrackExpanded> Tracks { get; set; }
    }
}