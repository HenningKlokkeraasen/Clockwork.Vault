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

        internal IList<string> Run(AlbumModel album, ICollection<ArtistModel> insertedArtists)
        {
            var log = new List<string>();
            
            // The album
            _saveTidalEntityHandler.MapAndUpsertAlbum(album);
            log.Add($"Saved album {album.Title}");

            // The main artist of the album
            if (album.Artist != null)
            {
                var savedArtist = SaveArtist(insertedArtists, album.Artist);
                if (savedArtist)
                    log.Add($"\tSaved artist {album.Artist.Name}");
            }

            // The artists of the album if multiple
            foreach (var albumArtist in album.Artists ?? Enumerable.Empty<ArtistModel>())
            {
                var savedArtistN = SaveArtist(insertedArtists, albumArtist);
                if (savedArtistN)
                    log.Add($"\tSaved artist {albumArtist.Name}");
            }

            // many-to-many table AlbumArtists
            _saveTidalEntityHandler.MapAndInsertAlbumArtists(album);

            return log;
        }
    }
}