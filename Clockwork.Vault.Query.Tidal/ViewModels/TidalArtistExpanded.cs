using System.Collections;
using System.Collections.Generic;
using Clockwork.Vault.Dao.Models.Tidal;

namespace Clockwork.Vault.Query.Tidal.ViewModels
{
    public class TidalArtistExpanded
    {
        public TidalArtist Artist { get; set; }
        public ICollection<TidalAlbum> Albums { get; set; }
        public ICollection<TidalAlbum> EPs { get; set; }
        public ICollection<TidalAlbum> Singles { get; set; }
        public ICollection<TidalAlbum> OtherReleases { get; set; }
        public ICollection<TidalTrack> NonAlbumTracks { get; set; }
    }
}