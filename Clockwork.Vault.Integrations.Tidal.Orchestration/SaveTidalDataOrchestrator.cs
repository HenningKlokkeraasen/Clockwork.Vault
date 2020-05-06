using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Clockwork.Vault.Core.Models;
using Clockwork.Vault.Dao;
using Clockwork.Vault.Integrations.Tidal.Orchestration.EnsureGlobalIdentifier;
using Clockwork.Vault.Integrations.Tidal.Orchestration.SaveCommands;
using Clockwork.Vault.Integrations.Tidal.Orchestration.SavePaths;
using OpenTidl.Methods;
using OpenTidl.Models;

namespace Clockwork.Vault.Integrations.Tidal.Orchestration
{
    /// <summary>
    /// The main entry point for saving Tidal data.
    /// There are four user data sets that can be saved:
    /// Favorite Artists,
    /// Favorite Albums,
    /// Favorite Tracks,
    /// Favorite Playlists,
    /// and User Playlists.
    /// Storing only these main entities will not be enough to use the data outside of Tidal,
    /// each data set stipulates a path for other entities to be saved.
    /// They are:
    /// * Favorite Artists: Artist, FavoriteArtist
    /// * Favorite Albums: Album, AlbumTrack, Track, TrackArtistPath, AlbumArtistPath, FavoriteAlbum
    /// * Favorite Tracks: TrackPath, FavoriteTrack
    /// * Favorite Playlists: PlaylistPath, FavoritePlaylist
    /// * User Playlists: PlaylistPath
    ///
    /// The PlaylistPath: Playlist, Creator, PlaylistTrack, TrackPath
    /// The TrackPath: Track, TrackArtistPath, AlbumTrack, Album, AlbumArtistPath
    /// The TrackArtistPath: TrackArtist, Artist
    /// The AlbumArtistPath: AlbumArtist, Artist
    /// 
    /// </summary>
    public class SaveTidalDataOrchestrator
    {
        private readonly string _username;
        private readonly string _password;
        private OpenTidlSession _openTidlSession;
        private readonly VaultContext _vaultContext;
        private readonly TidalIntegrator _tidalIntegrator;
        private readonly SaveTidalEntityHandler _saveTidalEntityHandler;

        public SaveTidalDataOrchestrator(string token, string username, string password)
        {
            _username = username;
            _password = password;
            _tidalIntegrator = new TidalIntegrator(token);
            _vaultContext = new VaultContext();
            _saveTidalEntityHandler = new SaveTidalEntityHandler(_vaultContext);
        }

        public async Task<Log> SavePlaylists(int limit = 9999)
        {
            await Init();

            var playlistsResult = await _openTidlSession.GetUserPlaylists(limit);
            var log = await new SavePlaylistsPath(_saveTidalEntityHandler, _openTidlSession, _tidalIntegrator, _vaultContext)
                .Run(playlistsResult.Items);

            return MakeLog("Tidal: Save User Playlists", log);
        }

        public async Task<Log> SaveUserFavPlaylists(int limit = 9999)
        {
            await Init();

            var favsResult = await _openTidlSession.GetFavoritePlaylists(limit);
            var items = favsResult.Items.Select(i => i.Item).ToList();
            var log = await new SavePlaylistsPath(_saveTidalEntityHandler, _openTidlSession, _tidalIntegrator, _vaultContext)
                .Run(items);

            // Favorite
            _saveTidalEntityHandler.MapAndInsertPlaylistFavorites(favsResult.Items);

            return MakeLog("Tidal: Save Fav Playlists", log);
        }

        public async Task<Log> SaveUserFavTracks(int limit = 9999)
        {
            await Init();

            var favsResult = await _openTidlSession.GetFavoriteTracks(limit);
            var items = favsResult.Items.Select(i => i.Item).ToList();

            var insertedAlbums = new List<AlbumModel>();
            var insertedArtists = new List<ArtistModel>();

            // Tracks with Albums and Artist(s)
            var saveTrackPath = new SaveTrackPath(_saveTidalEntityHandler, _tidalIntegrator, _vaultContext);
            var (_, log) = await saveTrackPath.Run(items, insertedAlbums, insertedArtists);

            // Favorite
            _saveTidalEntityHandler.MapAndInsertTrackFavorites(favsResult.Items);

            return MakeLog("Tidal: Save Fav Tracks", log);
        }

        public async Task<Log> SaveUserFavAlbums(int limit = 9999)
        {
            await Init();

            var log = new List<string>();

            var favsResult = await _openTidlSession.GetFavoriteAlbums(limit);
            var items = favsResult.Items.Select(i => i);

            var insertedArtists = new List<ArtistModel>();

            var albumWithArtistsPath = new SaveAlbumWithArtistsPath(_saveTidalEntityHandler);

            foreach (var jsonListItem in items)
            {
                var album = jsonListItem.Item;

                // Album, Artist(s) of the Album, and AlbumArtists
                var albumLog = albumWithArtistsPath.Run(album, insertedArtists);
                log.AddRange(albumLog);

                // Tracks, Artist(s) of the Track, TrackArtists, and AlbumTracks
                var tracks = await _tidalIntegrator.GetAlbumTracks(album.Id);
                if (tracks == null)
                    log.Add($"ERROR Could not get album tracks for album {album.Title} ({album.Id})");
                else
                {
                    var albumTracksLog = new SaveAlbumTracksPath(_saveTidalEntityHandler).Run(tracks.Items, album, insertedArtists);
                    log.AddRange(albumTracksLog.Select(l => $"\t{l}"));
                }

                // Favorite
                _saveTidalEntityHandler.MapAndInsertAlbumFavorite(jsonListItem);
            }

            return MakeLog("Tidal: Save Fav Albums", log);
        }

        public async Task<Log> SaveUserFavArtists(int limit = 9999)
        {
            await Init();

            var log = new List<string>();

            var favsResult = await _openTidlSession.GetFavoriteArtists(limit);
            var items = favsResult.Items.Select(i => i.Item).ToList();

            foreach (var item in items)
                _saveTidalEntityHandler.MapAndInsertArtist(item);

            _saveTidalEntityHandler.MapAndInsertArtistFavorites(favsResult.Items);

            items.ForEach(i => log.Add($"Saved artist {i.Name}"));

            return MakeLog("Tidal: Save Fav Artists", log);
        }

        public async Task<Log> EnsureAlbumUpc(IterationSettings iterationSettings)
        {
            await Init();
            var log = await new EnsureAlbumUpcHandler(_tidalIntegrator, _vaultContext).Run(iterationSettings);
            return MakeLog("Tidal: Ensure Album UPC", log);
        }

        public async Task<Log> EnsureTrackIsrc(IterationSettings iterationSettings)
        {
            await Init();
            var log = await new EnsureTrackIsrcHandler(_tidalIntegrator, _vaultContext).Run(iterationSettings);
            return MakeLog("Tidal: Ensure Track ISRC", log);
        }

        private async Task Init()
        {
            await LoginIfNotInInMemSession();
        }

        private async Task LoginIfNotInInMemSession()
        {
            // TODO thread-safe
            if (_openTidlSession == null)
                _openTidlSession = await _tidalIntegrator.LoginUserAsync(_username, _password);
        }

        private static Log MakeLog(string title, IList<string> log) => new Log
        {
            Title = title,
            Messages = log
        };
    }
}