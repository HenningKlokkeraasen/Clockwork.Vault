using System.Collections.Generic;
using System.Linq;
using Clockwork.Vault.Dao.Models.Tidal;

namespace Clockwork.Vault.Query.Tidal.Core
{
    public static class TidalReleaseExtensions
    {
        public static IEnumerable<TidalAlbum> SelectAlbums(this IEnumerable<TidalAlbum> releases)
            => releases.Where(a => a.Type == TidalConstants.AlbumTypes.Album);

        public static IEnumerable<TidalAlbum> SelectEps(this IEnumerable<TidalAlbum> releases)
            => releases.Where(a => a.Type == TidalConstants.AlbumTypes.Ep);

        public static IEnumerable<TidalAlbum> SelectSingles(this IEnumerable<TidalAlbum> releases)
            => releases.Where(a => a.Type == TidalConstants.AlbumTypes.Single);

        public static IEnumerable<TidalAlbum> SelectReleasesOfUndefinedType(this IEnumerable<TidalAlbum> releases)
            => releases.Where(a => string.IsNullOrEmpty(a.Type));
    }
}