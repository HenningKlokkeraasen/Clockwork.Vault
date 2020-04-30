using System.Collections.Generic;
using Clockwork.Vault.Dao.Models.Tidal;

namespace Clockwork.Vault.Query.Tidal.ViewModels
{
    public class TidalTrackExpanded
    {
        public TidalTrack Track { get; set; }
        public IList<TidalArtist> Artists { get; set; }
        public TidalAlbum Album { get; set; }
        public IList<TidalPlaylist> Playlists { get; set; }
    }
}