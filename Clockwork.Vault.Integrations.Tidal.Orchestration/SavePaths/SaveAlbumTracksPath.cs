using System.Collections.Generic;
using System.Linq;
using Clockwork.Vault.Integrations.Tidal.Orchestration.SaveCommands;
using OpenTidl.Models;

namespace Clockwork.Vault.Integrations.Tidal.Orchestration.SavePaths
{
    internal class SaveAlbumTracksPath : SavePathBase
    {
        internal SaveAlbumTracksPath(SaveTidalEntityHandler saveTidalEntityHandler)
            : base(saveTidalEntityHandler)
        {
        }

        internal IList<string> Run(IList<TrackModel> tracks, AlbumModel item, ICollection<ArtistModel> insertedArtists)
        {
            var log = new List<string>();

            // The tracks
            var (tidalTracks, tracksLog) = _saveTidalEntityHandler.MapAndUpsertTracks(tracks);
            log.AddRange(tracksLog);
            log.AddRange(tidalTracks.Select(t => $"\tSaved track {t.Title}"));

            foreach (var track in tracks)
            {
                // Artist(s) of the Track and TrackArtists
                var saveTrackArtistPath = new SaveTrackArtistPath(_saveTidalEntityHandler);
                saveTrackArtistPath.Run(track, insertedArtists);
            }

            // One-to-many table AlbumTracks
            _saveTidalEntityHandler.MapAndInsertAlbumTracks(tracks, item);

            return log;
        }
    }
}