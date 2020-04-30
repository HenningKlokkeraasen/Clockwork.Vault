using System.Collections.Generic;
using Clockwork.Vault.Dao.Models.Tidal;

namespace Clockwork.Vault.Query.Tidal.ViewModels.Grouped
{
    public class TidalPlaylistsGrouped
    {
        public string Creator { get; set; }
        public IList<TidalPlaylist> Playlists { get; set; }
    }
}