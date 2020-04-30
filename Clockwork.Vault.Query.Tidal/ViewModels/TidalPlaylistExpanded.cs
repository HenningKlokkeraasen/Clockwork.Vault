using System.Collections.Generic;
using Clockwork.Vault.Dao.Models.Tidal;

namespace Clockwork.Vault.Query.Tidal.ViewModels
{
    public class TidalPlaylistExpanded
    {
        public TidalPlaylist Playlist { get; set; }
        public ICollection<(int, TidalTrackExpanded)> Tracks { get; set; }

        public string Creator { get; set; }
    }
}