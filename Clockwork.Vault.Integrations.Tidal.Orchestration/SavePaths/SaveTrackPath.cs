using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Clockwork.Vault.Dao;
using Clockwork.Vault.Dao.Models.Tidal;
using Clockwork.Vault.Integrations.Tidal.Orchestration.SaveCommands;
using OpenTidl.Models;

namespace Clockwork.Vault.Integrations.Tidal.Orchestration.SavePaths
{
    internal class SaveTrackPath : SavePathBase
    {
        private readonly TidalIntegrator _tidalIntegrator;
        private readonly VaultContext _vaultContext;

        internal SaveTrackPath(SaveTidalEntityHandler saveTidalEntityHandler, TidalIntegrator tidalIntegrator, VaultContext vaultContext)
            : base(saveTidalEntityHandler)
        {
            _tidalIntegrator = tidalIntegrator;
            _vaultContext = vaultContext;
        }

        internal async Task<(IList<TidalTrack>, IList<string>)> Run(IList<TrackModel> tidalTracks,
            ICollection<AlbumModel> insertedAlbums, ICollection<ArtistModel> insertedArtists)
        {
            var log = new List<string>();

            // Tracks
            var (tracks, tracksLog) = _saveTidalEntityHandler.MapAndUpsertTracks(tidalTracks);
            log.AddRange(tracksLog);
            log.AddRange(tracks.Select(t => $"Saved track {t.Title}"));

            foreach (var track in tidalTracks)
            {
                // Album, Artist(s) of the Album, and AlbumArtists
                var saveAlbumTrackPath = new SaveAlbumWithArtistsPathExtended(_saveTidalEntityHandler, _tidalIntegrator, _vaultContext);
                var (_, saveAlbumTrackLog) = await saveAlbumTrackPath.Run(insertedAlbums, insertedArtists, track.Album);
                log.AddRange(saveAlbumTrackLog);
                
                // Artist(s) of the Track and TrackArtists
                var saveTrackArtistPath = new SaveTrackArtistPath(_saveTidalEntityHandler);
                var saveTrackArtistLog = saveTrackArtistPath.Run(track, insertedArtists);
                log.AddRange(saveTrackArtistLog);

                // one-to-many table AlbumTracks
                _saveTidalEntityHandler.MapAndInsertAlbumTrack(track);
            }

            return (tracks, log);
        }
    }
}