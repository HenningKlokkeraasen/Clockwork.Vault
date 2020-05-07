using System.Collections.Generic;
using Clockwork.Vault.Dao.Models.Tidal;

namespace Clockwork.Vault.Query.Tidal.ViewModels
{
    public class TidalTrackExpanded
    {
        public TidalTrack Track { get; set; }
        public IList<TidalArtist> MainArtists { get; set; }
        public IList<TidalArtist> FeaturedArtists { get; set; }
        public TidalAlbum Album { get; set; }
        public IList<TidalPlaylist> Playlists { get; set; }
    }
}