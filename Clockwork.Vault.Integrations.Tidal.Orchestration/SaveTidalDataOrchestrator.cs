using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Clockwork.Vault.Core.Models;
using Clockwork.Vault.Dao;
using Clockwork.Vault.Dao.Models.Tidal;
using OpenTidl.Methods;
using OpenTidl.Models;
using OpenTidl.Models.Base;

namespace Clockwork.Vault.Integrations.Tidal.Orchestration
{
    public class SaveTidalDataOrchestrator
    {
        private readonly string _username;
        private readonly string _password;
        private OpenTidlSession _openTidlSession;
        private VaultContext _vaultContext;
        private readonly TidalIntegrator _tidalIntegrator;

        private IList<string> Log;

        public SaveTidalDataOrchestrator(string token, string username, string password)
        {
            _username = username;
            _password = password;
            _tidalIntegrator = new TidalIntegrator(token);
        }

        public async Task<Log> SavePlaylists(int limit = 9999)
        {
            await Init(MethodBase.GetCurrentMethod().Name);

            var playlistsResult = await _openTidlSession.GetUserPlaylists(limit);
            await SavePlaylistsAndTracksAndAlbumsAndArtists(playlistsResult.Items);

            return MakeLog("Tidal: Save Playlists");
        }

        public async Task<Log> SaveUserFavPlaylists(int limit = 9999)
        {
            await Init(MethodBase.GetCurrentMethod().Name);

            var favsResult = await _openTidlSession.GetFavoritePlaylists(limit);
            var items = favsResult.Items.Select(i => i.Item).ToList();
            await SavePlaylistsAndTracksAndAlbumsAndArtists(items);
            MapAndInsertPlaylistFavorites(favsResult.Items);

            return MakeLog("Tidal: Save User Fav Playlists");
        }

        public async Task<Log> SaveUserFavTracks(int limit = 9999)
        {
            await Init(MethodBase.GetCurrentMethod().Name);

            var favsResult = await _openTidlSession.GetFavoriteTracks(limit);
            var items = favsResult.Items.Select(i => i.Item).ToList();

            var tracks = MapAndInsertTracks(items);

            var insertedAlbums = new List<AlbumModel>();
            var insertedArtists = new List<ArtistModel>();

            await GetAlbumsAndSaveAlbumsAndArtists(items, insertedAlbums, insertedArtists);
            MapAndInsertTrackFavorites(favsResult.Items);

            return MakeLog("Tidal: Save User Fav Tracks");
        }

        public async Task<Log> SaveUserFavAlbums(int limit = 9999)
        {
            await Init(MethodBase.GetCurrentMethod().Name);

            var favsResult = await _openTidlSession.GetFavoriteAlbums(limit);
            var items = favsResult.Items.Select(i => i.Item).ToList();

            var insertedArtists = new List<ArtistModel>();

            foreach (var item in items)
                SaveAlbumAndArtists(insertedArtists, item);

            foreach (var item in items)
                await SaveAlbumTracks(item);

            MapAndInsertAlbumFavorites(favsResult.Items);

            return MakeLog("Tidal: Save User Fav Albums");
        }

        public async Task<Log> SaveUserFavArtists(int limit = 9999)
        {
            await Init(MethodBase.GetCurrentMethod().Name);

            var favsResult = await _openTidlSession.GetFavoriteArtists(limit);
            var items = favsResult.Items.Select(i => i.Item).ToList();

            foreach (var item in items)
                MapAndInsertArtist(item);

            MapAndInsertArtistFavorites(favsResult.Items);

            return MakeLog("Tidal: Save User Fav Artists");
        }

        private async Task SaveAlbumTracks(AlbumModel item)
        {
            var tracks = await _tidalIntegrator.GetAlbumTracks(item.Id);
            if (tracks == null)
            {
                Console.WriteLine($"Could not get album tracks for album {item.Title} ({item.Id})");
                Log.Add($"ERROR Could not get album tracks for album {item.Title} ({item.Id})");
                return;
            }

            MapAndInsertTracks(tracks.Items);

            foreach (var track in tracks.Items)
            {
                var insertedArtists = new List<ArtistModel>();

                SaveArtist(insertedArtists, track.Artist);

                foreach (var trackArtist in track.Artists)
                    SaveArtist(insertedArtists, trackArtist);

                MapAndInsertTrackArtists(track);
            }

            MapAndInsertAlbumTracks(tracks.Items, item);
        }

        public async Task<Log> EnsureAlbumUpc(IterationSettings iterationSettings)
        {
            await Init(MethodBase.GetCurrentMethod().Name);

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
            await Init(MethodBase.GetCurrentMethod().Name);

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

        private async Task SavePlaylistsAndTracksAndAlbumsAndArtists(IList<PlaylistModel> playlistsItems)
        {
            MapAndInsertCreators(playlistsItems);

            var playlists = MapAndInsertPlaylists(playlistsItems);

            var insertedAlbums = new List<AlbumModel>();
            var insertedArtists = new List<ArtistModel>();

            foreach (var playlist in playlists)
            {
                var tracksResult = await _openTidlSession.GetPlaylistTracks(playlist.Uuid);

                var tracks = MapAndInsertTracks(tracksResult.Items);
                MapAndInsertPlaylistTracks(tracks, playlist);
                await GetAlbumsAndSaveAlbumsAndArtists(tracksResult.Items, insertedAlbums, insertedArtists);
            }
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

                MapAndInsertTrackArtists(track);
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

            MapAndInsertAlbumArtists(albumResult);
            return albumResult;
        }

        private void SaveAlbumAndArtists(ICollection<ArtistModel> insertedArtists, AlbumModel item)
        {
            MapAndInsertAlbum(item);

            SaveArtist(insertedArtists, item.Artist);

            foreach (var albumArtist in item.Artists ?? Enumerable.Empty<ArtistModel>())
                SaveArtist(insertedArtists, albumArtist);

            MapAndInsertAlbumArtists(item);
        }

        private void SaveArtist(ICollection<ArtistModel> insertedArtists, ArtistModel item)
        {
            if (insertedArtists == null || item == null)
                return;
            
            if (insertedArtists.All(a => a.Id != item.Id))
            {
                MapAndInsertArtist(item);
                insertedArtists.Add(item);
            }
        }

        private void MapAndInsertCreators(IEnumerable<PlaylistModel> result)
        {
            var creators = result.Select(TidalDaoMapper.MapTidalCreatorModelToDao);

            var distinctCreators = creators
                .GroupBy(c => c.Id)
                .Select(group => group.First())
                .ToList();

            distinctCreators.ForEach(c => TidalDbInserter.InsertCreator(_vaultContext, c));

            _vaultContext.SaveChanges();
        }

        private IEnumerable<TidalPlaylist> MapAndInsertPlaylists(IEnumerable<PlaylistModel> result)
        {
            var playlists = result.Select(TidalDaoMapper.MapTidalPlaylistModelToDao).ToList();

            playlists.ForEach(p => TidalDbInserter.InsertPlaylist(_vaultContext, p));

            _vaultContext.SaveChanges();

            return playlists;
        }

        private IEnumerable<TidalTrack> MapAndInsertTracks(IEnumerable<TrackModel> result)
        {
            var tracks = result.Select(TidalDaoMapper.MapTidalTrackModelToDao).ToList();

            var trackGroups = tracks.GroupBy(t => t.Id).ToList();

            trackGroups.Where(group => group.Count() > 1)
                .ToList()
                .ForEach(group => Log.Add($"WARN Duplicate track {group.Key} {group.First().Title}"));

            var distinctTracks = trackGroups
                .Select(group => group.First())
                .ToList();

            distinctTracks.ForEach(t => TidalDbInserter.UpsertTrack(_vaultContext, t));

            _vaultContext.SaveChanges();

            return distinctTracks;
        }

        private void MapAndInsertAlbumTracks(IEnumerable<TrackModel> tracks, AlbumModel album)
        {
            var position = 1;
            var albumTracks = tracks.Select(i => TidalDaoMapper.MapTidalAlbumTrackDao(i.Id, album.Id, position++))
                .ToList();

            albumTracks.ForEach(pt => TidalDbInserter.InsertAlbumTrack(_vaultContext, pt));

            _vaultContext.SaveChanges();
        }

        private void MapAndInsertPlaylistTracks(IEnumerable<TidalTrack> tracks, TidalPlaylist playlist)
        {
            var position = 1;
            var playlistTracks = tracks.Select(i => TidalDaoMapper.MapTidalPlaylistTrackDao(i.Id, playlist.Uuid, position++))
                .ToList();

            playlistTracks.ForEach(pt => TidalDbInserter.InsertPlaylistTrack(_vaultContext, pt));

            _vaultContext.SaveChanges();
        }

        private TidalAlbum MapAndInsertAlbum(AlbumModel result)
        {
            var album = TidalDaoMapper.MapTidalAlbumModelToDao(result);

            TidalDbInserter.UpsertAlbum(_vaultContext, album);

            _vaultContext.SaveChanges();

            return album;
        }

        private TidalArtist MapAndInsertArtist(ArtistModel result)
        {
            var artist = TidalDaoMapper.MapTidalArtistModelToDao(result);

            TidalDbInserter.InsertArtist(_vaultContext, artist);

            _vaultContext.SaveChanges();

            return artist;
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

            MapAndInsertAlbum(albumResult);

            return albumResult;
        }

        private void MapAndInsertTrackArtists(TrackModel track)
        {
            if (track?.Artist != null)
            {
                var trackArtist = TidalDaoMapper.MapTidalTrackArtistDao(track, track.Artist);
                TidalDbInserter.InsertTrackArtist(_vaultContext, trackArtist);
            }

            if (track?.Artists != null)
            {
                var trackArtists = track.Artists.Select(i => TidalDaoMapper.MapTidalTrackArtistDao(track, i));

                foreach (var trackArtist in trackArtists)
                    TidalDbInserter.InsertTrackArtist(_vaultContext, trackArtist);
            }

            _vaultContext.SaveChanges();
        }

        private void MapAndInsertAlbumArtists(AlbumModel album)
        {
            if (album?.Artist != null)
            {
                var albumArtist = TidalDaoMapper.MapTidalAlbumArtistDao(album, album.Artist);
                TidalDbInserter.InsertAlbumArtist(_vaultContext, albumArtist);
            }

            if (album?.Artists != null)
            {
                var albumArtists = album.Artists.Select(i => TidalDaoMapper.MapTidalAlbumArtistDao(album, i));

                foreach (var albumArtist in albumArtists)
                    TidalDbInserter.InsertAlbumArtist(_vaultContext, albumArtist);
            }

            _vaultContext.SaveChanges();
        }

        private void MapAndInsertPlaylistFavorites(IEnumerable<JsonListItem<PlaylistModel>> jsonListItems)
        {
            var favs = jsonListItems.Select(TidalDaoMapper.MapTidalPlaylistFavDao);

            foreach (var fav in favs)
                TidalDbInserter.InsertFavPlaylist(_vaultContext, fav);

            _vaultContext.SaveChanges();
        }

        private void MapAndInsertAlbumFavorites(IEnumerable<JsonListItem<AlbumModel>> jsonListItems)
        {
            var favs = jsonListItems.Select(TidalDaoMapper.MapTidalAlbumFavDao);

            foreach (var fav in favs)
                TidalDbInserter.InsertFavAlbum(_vaultContext, fav);

            _vaultContext.SaveChanges();
        }

        private void MapAndInsertTrackFavorites(IEnumerable<JsonListItem<TrackModel>> jsonListItems)
        {
            var favs = jsonListItems.Select(TidalDaoMapper.MapTidalTrackFavDao);

            foreach (var fav in favs)
                TidalDbInserter.InsertFavTrack(_vaultContext, fav);

            _vaultContext.SaveChanges();
        }

        private void MapAndInsertArtistFavorites(IEnumerable<JsonListItem<ArtistModel>> jsonListItems)
        {
            var favs = jsonListItems.Select(TidalDaoMapper.MapTidalArtistFavDao);

            foreach (var fav in favs)
                TidalDbInserter.InsertFavArtist(_vaultContext, fav);

            _vaultContext.SaveChanges();
        }

        private async Task Init(string nameOfCallingMethod)
        {
            // TODO thread-safe
            Log = new List<string>();

            Log.Add($"Starting {nameOfCallingMethod}");
            GetInMemContextOrEstablish();
            await LoginIfNotInInMemSession();
        }

        private void GetInMemContextOrEstablish()
        {
            if (_vaultContext == null)
                _vaultContext = new VaultContext();
        }

        private async Task LoginIfNotInInMemSession()
        {
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
