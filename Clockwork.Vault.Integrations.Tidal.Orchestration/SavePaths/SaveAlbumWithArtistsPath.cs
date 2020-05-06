using System.Collections.Generic;
using System.Linq;
using Clockwork.Vault.Integrations.Tidal.Orchestration.SaveCommands;
using OpenTidl.Models;

namespace Clockwork.Vault.Integrations.Tidal.Orchestration.SavePaths
{
    internal class SaveAlbumWithArtistsPath : SavePathBase
    {
        internal SaveAlbumWithArtistsPath(SaveTidalEntityHandler saveTidalEntityHandler)
            : base(saveTidalEntityHandler)
        {
        }

        internal IList<string> Run(AlbumModel item, ICollection<ArtistModel> insertedArtists)
        {
            var log = new List<string>();
            
            // The album
            _saveTidalEntityHandler.MapAndUpsertAlbum(item);
            log.Add($"Saved album {item.Title}");

            // The main artist of the album
            if (item.Artist != null)
            {
                var savedArtist = SaveArtist(insertedArtists, item.Artist);
                if (savedArtist)
                    log.Add($"\tSaved artist {item.Artist.Name}");
            }

            // The artists of the album if multiple
            foreach (var albumArtist in item.Artists ?? Enumerable.Empty<ArtistModel>())
            {
                var savedArtistN = SaveArtist(insertedArtists, albumArtist);
                if (savedArtistN)
                    log.Add($"\tSaved artist {albumArtist.Name}");
            }

            // many-to-many table AlbumArtists
            _saveTidalEntityHandler.MapAndInsertAlbumArtists(item);

            return log;
        }
    }
}