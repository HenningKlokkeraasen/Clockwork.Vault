using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Clockwork.Vault.Core.Models;
using Clockwork.Vault.Dao;
using Clockwork.Vault.Integrations.Tidal.Orchestration.SaveCommands;
using Clockwork.Vault.Integrations.Tidal.Orchestration.SavePaths;
using OpenTidl.Methods;
using OpenTidl.Models;

namespace Clockwork.Vault.Integrations.Tidal.Orchestration
{
    public class SaveTidalDataOrchestrator
    {
        private readonly string _username;
        private readonly string _password;
        private OpenTidlSession _openTidlSession;
        private readonly VaultContext _vaultContext;
        private readonly TidalIntegrator _tidalIntegrator;
        private readonly SaveTidalEntityHandler _saveTidalEntityHandler;

        private List<string> Log;

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
            await SavePlaylistsAndTracksAndAlbumsAndArtists(playlistsResult.Items);

            return MakeLog("Tidal: Save Playlists");
        }

        public async Task<Log> SaveUserFavPlaylists(int limit = 9999)
        {
            await Init();

            var favsResult = await _openTidlSession.GetFavoritePlaylists(limit);
            var items = favsResult.Items.Select(i => i.Item).ToList();
            await SavePlaylistsAndTracksAndAlbumsAndArtists(items);
            _saveTidalEntityHandler.MapAndInsertPlaylistFavorites(favsResult.Items);

            return MakeLog("Tidal: Save User Fav Playlists");
        }

        public async Task<Log> SaveUserFavTracks(int limit = 9999)
        {
            await Init();

            var favsResult = await _openTidlSession.GetFavoriteTracks(limit);
            var items = favsResult.Items.Select(i => i.Item).ToList();

            var tracks = _saveTidalEntityHandler.MapAndUpsertTracks(items);

            var insertedAlbums = new List<AlbumModel>();
            var insertedArtists = new List<ArtistModel>();

            await GetAlbumsAndSaveAlbumsAndArtists(items, insertedAlbums, insertedArtists);
            _saveTidalEntityHandler.MapAndInsertTrackFavorites(favsResult.Items);

            return MakeLog("Tidal: Save User Fav Tracks");
        }

        public async Task<Log> SaveUserFavAlbums(int limit = 9999)
        {
            await Init();

            var favsResult = await _openTidlSession.GetFavoriteAlbums(limit);
            var items = favsResult.Items.Select(i => i);

            var insertedArtists = new List<ArtistModel>();

            var saveUserFavAlbumsPath = new SaveUserFavAlbumsPath(_saveTidalEntityHandler);

            foreach (var jsonListItem in items)
            {
                var album = jsonListItem.Item;

                // Album, Artist(s) of the Album, and AlbumArtists
                var albumLog = saveUserFavAlbumsPath.SaveAlbumWithArtists(album, insertedArtists);
                Log.AddRange(albumLog);

                // Tracks, Artist(s) of the Track, TrackArtists, and AlbumTracks
                var tracks = await _tidalIntegrator.GetAlbumTracks(album.Id);
                if (tracks == null)
                    Log.Add($"ERROR Could not get album tracks for album {album.Title} ({album.Id})");
                else
                {
                    var albumTracksLog = saveUserFavAlbumsPath.SaveAlbumTracks(tracks.Items, album, insertedArtists);
                    Log.AddRange(albumTracksLog.Select(l => $"\t{l}"));
                }

                // Favorite
                _saveTidalEntityHandler.MapAndInsertAlbumFavorite(jsonListItem);
            }

            return MakeLog("Tidal: Save User Fav Albums");
        }

        public async Task<Log> SaveUserFavArtists(int limit = 9999)
        {
            await Init();

            var favsResult = await _openTidlSession.GetFavoriteArtists(limit);
            var items = favsResult.Items.Select(i => i.Item).ToList();

            foreach (var item in items)
                _saveTidalEntityHandler.MapAndInsertArtist(item);

            _saveTidalEntityHandler.MapAndInsertArtistFavorites(favsResult.Items);

            items.ForEach(i => Log.Add($"Saved artist {i.Name}"));

            return MakeLog("Tidal: Save User Fav Artists");
        }

        public async Task<Log> EnsureAlbumUpc(IterationSettings iterationSettings)
        {
            await Init();

            var queryable = from a in _vaultContext.TidalAlbums.Where(a => a.Upc == null)
                            select a;
            // https://stackoverflow.com/questions/2113498/sqlexception-from-entity-framework-new-transaction-is-not-allowed-because-ther
            var albumsWithoutUpc = queryable.ToList();

            var sleepTimeInSeconds = iterationSettings?.SleepTimeInSeconds > 0
                ? iterationSettings.SleepTimeInSeconds
                : 0;

            foreach (var tidalAlbum in albumsWithoutUpc)
            {
                var albumResult = await _tidalIntegrator.GetAlbum(tidalAlbum.Id);

                if (albumResult == null)
                {
                    Log.Add($"WARN Could not get album {tidalAlbum.Id} {tidalAlbum.Title}");
                }
                else
                {
                    var album = TidalDaoMapper.MapTidalAlbumModelToDao(albumResult);
                    TidalDbInserter.UpdateFields(_vaultContext, album, tidalAlbum);
                }

                Log.Add($"Sleeping for {sleepTimeInSeconds} seconds");
                Thread.Sleep(sleepTimeInSeconds * 1000);
            }

            return MakeLog("Tidal: Ensure Album UPC");
        }

        public async Task<Log> EnsureTrackIsrc(IterationSettings iterationSettings)
        {
            await Init();

            var queryable = from a in _vaultContext.TidalTracks.Where(a => a.Isrc == null)
                            select a;
            // https://stackoverflow.com/questions/2113498/sqlexception-from-entity-framework-new-transaction-is-not-allowed-because-ther
            var tracksWithoutIsrc = queryable.ToList();

            var sleepTimeInSeconds = iterationSettings?.SleepTimeInSeconds > 0
                ? iterationSettings.SleepTimeInSeconds
                : 0;

            foreach (var tidalTrack in tracksWithoutIsrc)
            {
                var trackResult = await _tidalIntegrator.GetTrack(tidalTrack.Id);

                if (trackResult == null)
                {
                    Log.Add($"WARN Could not get track {tidalTrack.Id} {tidalTrack.Title}");
                }
                else
                {
                    var album = TidalDaoMapper.MapTidalTrackModelToDao(trackResult);
                    TidalDbInserter.UpdateFields(_vaultContext, album, tidalTrack);
                }

                Log.Add($"Sleeping for {sleepTimeInSeconds} seconds");
                Thread.Sleep(sleepTimeInSeconds * 1000);
            }

            return MakeLog("Tidal: Ensure Track ISRC");
        }

        // Returns log
        private async Task<IList<string>> SavePlaylistsAndTracksAndAlbumsAndArtists(IList<PlaylistModel> playlistsItems)
        {
            var tracksLog = new List<string>();

            _saveTidalEntityHandler.MapAndInsertCreators(playlistsItems);

            var playlists = _saveTidalEntityHandler.MapAndInsertPlaylists(playlistsItems);

            var insertedAlbums = new List<AlbumModel>();
            var insertedArtists = new List<ArtistModel>();

            foreach (var playlist in playlists)
            {
                var tracksResult = await _openTidlSession.GetPlaylistTracks(playlist.Uuid);

                var (tracks, playlistTracksLog) = _saveTidalEntityHandler.MapAndUpsertTracks(tracksResult.Items);
                tracksLog.AddRange(playlistTracksLog);
                _saveTidalEntityHandler.MapAndInsertPlaylistTracks(tracks, playlist);
                await GetAlbumsAndSaveAlbumsAndArtists(tracksResult.Items, insertedAlbums, insertedArtists);
            }

            return tracksLog;
        }

        private async Task GetAlbumsAndSaveAlbumsAndArtists(IEnumerable<TrackModel> tracksItems,
             ICollection<AlbumModel> insertedAlbums, ICollection<ArtistModel> insertedArtists)
        {
            foreach (var track in tracksItems)
            {
                if (insertedAlbums.All(a => a.Id != track.Album.Id))
                {
                    var albumResult = await GetAlbumAndSaveAlbumAndArtists(insertedArtists, track.Album);
                    if (albumResult != null)
                        insertedAlbums.Add(albumResult);
                }

                SaveArtist(insertedArtists, track.Artist);

                foreach (var trackArtist in track.Artists)
                    SaveArtist(insertedArtists, trackArtist);

                _saveTidalEntityHandler.MapAndInsertTrackArtists(track);

                var albumTrack = TidalDaoMapper.MapTidalAlbumTrackDao(track.Id, track.Album.Id, 0);
                TidalDbInserter.InsertAlbumTrack(_vaultContext, albumTrack);
                _vaultContext.SaveChanges();
            }
        }

        private async Task<AlbumModel> GetAlbumAndSaveAlbumAndArtists(ICollection<ArtistModel> insertedArtists, AlbumModel item)
        {
            var existingRecord = _vaultContext.TidalAlbums.FirstOrDefault(p => p.Id == item.Id);
            if (existingRecord != null)
            {
                Log.Add($"Record exists: album {item.Id} {item.Title}");
                if (existingRecord.Upc != null)
                {
                    Log.Add("    album has UPC - will not get album or insert album or album artists");
                    return null;
                }
            }

            var albumResult = await GetAlbumAndMapAndInsert(item);
            if (albumResult == null)
                return null;

            SaveArtist(insertedArtists, albumResult.Artist);

            foreach (var albumArtist in albumResult.Artists ?? Enumerable.Empty<ArtistModel>())
                SaveArtist(insertedArtists, albumArtist);

            _saveTidalEntityHandler.MapAndInsertAlbumArtists(albumResult);
            return albumResult;
        }
        
        private void SaveArtist(ICollection<ArtistModel> insertedArtists, ArtistModel item)
        {
            if (insertedArtists == null || item == null)
                return;
            
            if (insertedArtists.All(a => a.Id != item.Id))
            {
                _saveTidalEntityHandler.MapAndInsertArtist(item);
                insertedArtists.Add(item);
            }
        }

        private async Task<AlbumModel> GetAlbumAndMapAndInsert(AlbumModel lesserAlbumModel)
        {
            var existingRecord = _vaultContext.TidalAlbums.FirstOrDefault(p => p.Id == lesserAlbumModel.Id);
            if (existingRecord != null)
            {
                Log.Add($"Record exists: album {existingRecord.Id} {lesserAlbumModel.Title}");
                if (existingRecord.Upc != null)
                {
                    Log.Add("    album has UPC - will not get album or insert album or album artists");
                    return null;
                }
            }

            var albumResult = await _tidalIntegrator.GetAlbum(lesserAlbumModel.Id);
            if (albumResult == null)
            {
                Log.Add($"WARN Could not get album {lesserAlbumModel.Id} {lesserAlbumModel.Title} - inserting lesser album model");
                Console.WriteLine($"WARN Could not get album {lesserAlbumModel.Id} {lesserAlbumModel.Title} - inserting lesser album model");
                albumResult = lesserAlbumModel;
            }

            _saveTidalEntityHandler.MapAndUpsertAlbum(albumResult);

            return albumResult;
        }

        private async Task Init()
        {
            // TODO thread-safe
            Log = new List<string>();

            await LoginIfNotInInMemSession();
        }

        private async Task LoginIfNotInInMemSession()
        {
            // TODO thread-safe
            if (_openTidlSession == null)
                _openTidlSession = await _tidalIntegrator.LoginUserAsync(_username, _password);
        }

        private Log MakeLog(string title) => new Log
        {
            Title = title,
            Messages = Log
        };
    }
}
