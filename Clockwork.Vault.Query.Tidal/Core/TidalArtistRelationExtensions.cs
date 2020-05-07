using System.Collections.Generic;
using System.Linq;
using Clockwork.Vault.Dao.Models.Tidal;

namespace Clockwork.Vault.Query.Tidal.Core
{
    public static class TidalArtistRelationExtensions
    {
        public static IEnumerable<TidalArtist> SelectMainArtists(this IEnumerable<TidalArtistRelationBase> artistRelations)
            => artistRelations.Where(a => a.Type == TidalConstants.ArtistParticipationType.Main)
                .Select(a => a.Artist);

        public static IEnumerable<TidalArtist> SelectFeaturedArtists(this IEnumerable<TidalArtistRelationBase> artistRelations)
            => artistRelations.Where(a => a.Type == TidalConstants.ArtistParticipationType.Featured)
                .Select(a => a.Artist);
    }
}