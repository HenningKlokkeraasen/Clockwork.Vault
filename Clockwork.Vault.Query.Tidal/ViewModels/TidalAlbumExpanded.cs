using System.Collections.Generic;
using Clockwork.Vault.Dao.Models.Tidal;

namespace Clockwork.Vault.Query.Tidal.ViewModels
{
    public class TidalAlbumExpanded
    {
        public TidalAlbum Album { get; set; }
        public ICollection<TidalArtist> MainArtists { get; set; }
        public ICollection<TidalArtist> FeaturedArtists { get; set; }
        public ICollection<TidalTrackExpanded> Tracks { get; set; }
    }
}