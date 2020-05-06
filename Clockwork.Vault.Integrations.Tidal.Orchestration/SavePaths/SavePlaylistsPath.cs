using System.Collections.Generic;
using System.Threading.Tasks;
using Clockwork.Vault.Dao;
using Clockwork.Vault.Integrations.Tidal.Orchestration.SaveCommands;
using OpenTidl.Methods;
using OpenTidl.Models;

namespace Clockwork.Vault.Integrations.Tidal.Orchestration.SavePaths
{
    internal class SavePlaylistsPath : SavePathBase
    {
        private readonly OpenTidlSession _openTidlSession;
        private readonly TidalIntegrator _tidalIntegrator;
        private readonly VaultContext _vaultContext;

        internal SavePlaylistsPath(SaveTidalEntityHandler saveTidalEntityHandler,
            OpenTidlSession openTidlSession, TidalIntegrator tidalIntegrator, VaultContext vaultContext)
            : base(saveTidalEntityHandler)
        {
            _openTidlSession = openTidlSession;
            _tidalIntegrator = tidalIntegrator;
            _vaultContext = vaultContext;
        }

        internal async Task<IList<string>> Run(IList<PlaylistModel> playlistsItems)
        {
            var tracksLog = new List<string>();

            // Creators
            _saveTidalEntityHandler.MapAndInsertCreators(playlistsItems);

            // Playlists
            var playlists = _saveTidalEntityHandler.MapAndInsertPlaylists(playlistsItems);

            var insertedAlbums = new List<AlbumModel>();
            var insertedArtists = new List<ArtistModel>();

            foreach (var playlist in playlists)
            {
                var tracksResult = await _openTidlSession.GetPlaylistTracks(playlist.Uuid);

                // Tracks with Albums and Artist(s)
                var saveTrackPath = new SaveTrackPath(_saveTidalEntityHandler, _tidalIntegrator, _vaultContext);
                var (tracks, log) = await saveTrackPath.Run(tracksResult.Items, insertedAlbums, insertedArtists);
                tracksLog.AddRange(log);

                // PlaylistTracks
                _saveTidalEntityHandler.MapAndInsertPlaylistTracks(tracks, playlist);
            }

            return tracksLog;
        }
    }
}