using System.Collections.Generic;
using System.Linq;
using Clockwork.Vault.Integrations.Tidal.Orchestration.SaveCommands;
using OpenTidl.Models;

namespace Clockwork.Vault.Integrations.Tidal.Orchestration.SavePaths
{
    internal class SaveUserFavAlbumsPath : SavePathBase
    {
        internal SaveUserFavAlbumsPath(SaveTidalEntityHandler saveTidalEntityHandler)
            : base(saveTidalEntityHandler)
        {
        }

        internal IList<string> SaveAlbumWithArtists(AlbumModel item, ICollection<ArtistModel> insertedArtists)
        {
            var log = new List<string>();

            // The album
            _saveTidalEntityHandler.MapAndUpsertAlbum(item);
            log.Add($"Saved album {item.Title}");

            // The main artist of the album
            var savedArtist = SaveArtist(insertedArtists, item.Artist);
            if (savedArtist)
                log.Add($"\tSaved artist {item.Artist.Name}");

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

        internal IList<string> SaveAlbumTracks(IList<TrackModel> tracks, AlbumModel item, ICollection<ArtistModel> insertedArtists)
        {
            var log = new List<string>();

            // The tracks
            var (tidalTracks, tracksLog) = _saveTidalEntityHandler.MapAndUpsertTracks(tracks);
            log.AddRange(tracksLog);
            log.AddRange(tidalTracks.Select(t => $"\tSaved track {t.Title}"));

            foreach (var track in tracks)
            {
                // The main artist of the track
                var savedArtist = SaveArtist(insertedArtists, track.Artist);
                if (savedArtist)
                    log.Add($"\t\tSaved artist {item.Artist.Name}");

                // The artists of the track if multiple
                foreach (var trackArtist in track.Artists)
                {
                    var savedArtistN = SaveArtist(insertedArtists, trackArtist);
                    if (savedArtistN)
                        log.Add($"\t\tSaved artist {trackArtist.Name}");
                }

                // many-to-many table TrackArtists
                _saveTidalEntityHandler.MapAndInsertTrackArtists(track);
            }

            // many-to-many table AlbumTracks
            _saveTidalEntityHandler.MapAndInsertAlbumTracks(tracks, item);

            return log;
        }
    }
}