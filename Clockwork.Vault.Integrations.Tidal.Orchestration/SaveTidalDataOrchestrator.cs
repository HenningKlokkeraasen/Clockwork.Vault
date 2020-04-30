using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Clockwork.Vault.Dao;
using Clockwork.Vault.Dao.Models.Tidal;
using log4net;
using OpenTidl.Methods;
using OpenTidl.Models;
using OpenTidl.Models.Base;

namespace Clockwork.Vault.Integrations.Tidal.Orchestration
{
    public class SaveTidalDataOrchestrator
    {
        private readonly TidalIntegrator _tidalIntegrator;

        private static readonly ILog Log = LogManager.GetLogger("Default");

        public SaveTidalDataOrchestrator(string token)
        {
            _tidalIntegrator = new TidalIntegrator(token);
        }

        public async Task SavePlaylists(OpenTidlSession session, VaultContext context, int limit = 9999)
        {
            var playlistsResult = await session.GetUserPlaylists(limit);
            await SavePlaylistsAndTracksAndAlbumsAndArtists(session, context, playlistsResult.Items);
        }

        public async Task SaveUserFavPlaylists(OpenTidlSession session, VaultContext context, int limit = 9999)
        {
            var favsResult = await session.GetFavoritePlaylists(limit);
            var items = favsResult.Items.Select(i => i.Item).ToList();
            await SavePlaylistsAndTracksAndAlbumsAndArtists(session, context, items);
            MapAndInsertPlaylistFavorites(context, favsResult.Items);
        }

        public async Task SaveUserFavTracks(OpenTidlSession session, VaultContext context, int limit = 9999)
        {
            var favsResult = await session.GetFavoriteTracks(limit);
            var items = favsResult.Items.Select(i => i.Item).ToList();

            var tracks = MapAndInsertTracks(context, items);

            var insertedAlbums = new List<AlbumModel>();
            var insertedArtists = new List<ArtistModel>();

            await GetAlbumsAndSaveAlbumsAndArtists(context, items, insertedAlbums, insertedArtists);
            MapAndInsertTrackFavorites(context, favsResult.Items);
        }

        public async Task SaveUserFavAlbums(OpenTidlSession session, VaultContext context, int limit = 9999)
        {
            var favsResult = await session.GetFavoriteAlbums(limit);
            var items = favsResult.Items.Select(i => i.Item).ToList();

            var insertedArtists = new List<ArtistModel>();

            foreach (var item in items)
                SaveAlbumAndArtists(context, insertedArtists, item);

            foreach (var item in items)
                await SaveAlbumTracks(context, item);

            MapAndInsertAlbumFavorites(context, favsResult.Items);
        }

        public async Task SaveUserFavArtists(OpenTidlSession session, VaultContext context, int limit = 9999)
        {
            var favsResult = await session.GetFavoriteArtists(limit);
            var items = favsResult.Items.Select(i => i.Item).ToList();

            foreach (var item in items)
                MapAndInsertArtist(context, item);

            MapAndInsertArtistFavorites(context, favsResult.Items);
        }

        private async Task SaveAlbumTracks(VaultContext context, AlbumModel item)
        {
            var tracks = await _tidalIntegrator.GetAlbumTracks(item.Id);
            if (tracks == null)
            {
                Console.WriteLine($"Could not get album tracks for album {item.Title} ({item.Id})");
                Log.Error($"Could not get album tracks for album {item.Title} ({item.Id})");
                return;
            }

            MapAndInsertTracks(context, tracks.Items);

            foreach (var track in tracks.Items)
            {
                var insertedArtists = new List<ArtistModel>();

                SaveArtist(context, insertedArtists, track.Artist);

                foreach (var trackArtist in track.Artists)
                    SaveArtist(context, insertedArtists, trackArtist);

                MapAndInsertTrackArtists(context, track);
            }

            MapAndInsertAlbumTracks(context, tracks.Items, item);
        }

        public async Task EnsureAlbumUpc(VaultContext context, IterationSettings iterationSettings)
        {
            var queryable = from a in context.TidalAlbums.Where(a => a.Upc == null)
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
                    Log.Warn($"Could not get album {tidalAlbum.Id} {tidalAlbum.Title}");
                }
                else
                {
                    var album = TidalDaoMapper.MapTidalAlbumModelToDao(albumResult);
                    TidalDbInserter.UpdateFields(context, album, tidalAlbum);
                }

                Log.Info($"Sleeping for {sleepTimeInSeconds} seconds");
                Thread.Sleep(sleepTimeInSeconds * 1000);
            }
        }

        public async Task EnsureTrackIsrc(VaultContext context, IterationSettings iterationSettings)
        {
            var queryable = from a in context.TidalTracks.Where(a => a.Isrc == null)
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
                    Log.Warn($"Could not get track {tidalTrack.Id} {tidalTrack.Title}");
                }
                else
                {
                    var album = TidalDaoMapper.MapTidalTrackModelToDao(trackResult);
                    TidalDbInserter.UpdateFields(context, album, tidalTrack);
                }

                Log.Info($"Sleeping for {sleepTimeInSeconds} seconds");
                Thread.Sleep(sleepTimeInSeconds * 1000);
            }
        }

        private async Task SavePlaylistsAndTracksAndAlbumsAndArtists(OpenTidlSession session, VaultContext context, IList<PlaylistModel> playlistsItems)
        {
            MapAndInsertCreators(context, playlistsItems);

            var playlists = MapAndInsertPlaylists(context, playlistsItems);

            var insertedAlbums = new List<AlbumModel>();
            var insertedArtists = new List<ArtistModel>();

            foreach (var playlist in playlists)
            {
                var tracksResult = await session.GetPlaylistTracks(playlist.Uuid);

                var tracks = MapAndInsertTracks(context, tracksResult.Items);
                MapAndInsertPlaylistTracks(context, tracks, playlist);
                await GetAlbumsAndSaveAlbumsAndArtists(context, tracksResult.Items, insertedAlbums, insertedArtists);
            }
        }

        private async Task GetAlbumsAndSaveAlbumsAndArtists(VaultContext context, IEnumerable<TrackModel> tracksItems,
             ICollection<AlbumModel> insertedAlbums, ICollection<ArtistModel> insertedArtists)
        {
            foreach (var track in tracksItems)
            {
                if (insertedAlbums.All(a => a.Id != track.Album.Id))
                {
                    var albumResult = await GetAlbumAndSaveAlbumAndArtists(context, insertedArtists, track.Album);
                    if (albumResult != null)
                        insertedAlbums.Add(albumResult);
                }

                SaveArtist(context, insertedArtists, track.Artist);

                foreach (var trackArtist in track.Artists)
                    SaveArtist(context, insertedArtists, trackArtist);

                MapAndInsertTrackArtists(context, track);
            }
        }

        private async Task<AlbumModel> GetAlbumAndSaveAlbumAndArtists(VaultContext context,
            ICollection<ArtistModel> insertedArtists, AlbumModel item)
        {
            var existingRecord = context.TidalAlbums.FirstOrDefault(p => p.Id == item.Id);
            if (existingRecord != null)
            {
                Log.Info($"Record exists: album {item.Id} {item.Title}");
                if (existingRecord.Upc != null)
                {
                    Log.Info("    album has UPC - will not get album or insert album or album artists");
                    return null;
                }
            }

            var albumResult = await GetAlbumAndMapAndInsert(context, item);
            if (albumResult == null)
                return null;

            SaveArtist(context, insertedArtists, albumResult.Artist);

            foreach (var albumArtist in albumResult.Artists ?? Enumerable.Empty<ArtistModel>())
                SaveArtist(context, insertedArtists, albumArtist);

            MapAndInsertAlbumArtists(context, albumResult);
            return albumResult;
        }

        private void SaveAlbumAndArtists(VaultContext context, ICollection<ArtistModel> insertedArtists, AlbumModel item)
        {
            MapAndInsertAlbum(context, item);

            SaveArtist(context, insertedArtists, item.Artist);

            foreach (var albumArtist in item.Artists ?? Enumerable.Empty<ArtistModel>())
                SaveArtist(context, insertedArtists, albumArtist);

            MapAndInsertAlbumArtists(context, item);
        }

        private void SaveArtist(VaultContext context, ICollection<ArtistModel> insertedArtists, ArtistModel item)
        {
            if (insertedArtists == null || item == null)
                return;
            
            if (insertedArtists.All(a => a.Id != item.Id))
            {
                MapAndInsertArtist(context, item);
                insertedArtists.Add(item);
            }
        }

        private static void MapAndInsertCreators(VaultContext context, IEnumerable<PlaylistModel> result)
        {
            var creators = result.Select(TidalDaoMapper.MapTidalCreatorModelToDao);

            var distinctCreators = creators
                .GroupBy(c => c.Id)
                .Select(group => group.First())
                .ToList();

            distinctCreators.ForEach(c => TidalDbInserter.InsertCreator(context, c));

            context.SaveChanges();
        }

        private static IEnumerable<TidalPlaylist> MapAndInsertPlaylists(VaultContext context, IEnumerable<PlaylistModel> result)
        {
            var playlists = result.Select(TidalDaoMapper.MapTidalPlaylistModelToDao).ToList();

            playlists.ForEach(p => TidalDbInserter.InsertPlaylist(context, p));

            context.SaveChanges();

            return playlists;
        }

        private static IEnumerable<TidalTrack> MapAndInsertTracks(VaultContext context, IEnumerable<TrackModel> result)
        {
            var tracks = result.Select(TidalDaoMapper.MapTidalTrackModelToDao).ToList();

            var trackGroups = tracks.GroupBy(t => t.Id).ToList();

            trackGroups.Where(group => group.Count() > 1)
                .ToList()
                .ForEach(group => Log.Warn($"Duplicate track {group.Key} {group.First().Title}"));

            var distinctTracks = trackGroups
                .Select(group => group.First())
                .ToList();

            distinctTracks.ForEach(t => TidalDbInserter.UpsertTrack(context, t));

            context.SaveChanges();

            return distinctTracks;
        }

        private static void MapAndInsertAlbumTracks(VaultContext context, IEnumerable<TrackModel> tracks, AlbumModel album)
        {
            var position = 1;
            var albumTracks = tracks.Select(i => TidalDaoMapper.MapTidalAlbumTrackDao(i.Id, album.Id, position++))
                .ToList();

            albumTracks.ForEach(pt => TidalDbInserter.InsertAlbumTrack(context, pt));

            context.SaveChanges();
        }

        private static void MapAndInsertPlaylistTracks(VaultContext context, IEnumerable<TidalTrack> tracks, TidalPlaylist playlist)
        {
            var position = 1;
            var playlistTracks = tracks.Select(i => TidalDaoMapper.MapTidalPlaylistTrackDao(i.Id, playlist.Uuid, position++))
                .ToList();

            playlistTracks.ForEach(pt => TidalDbInserter.InsertPlaylistTrack(context, pt));

            context.SaveChanges();
        }

        private static TidalAlbum MapAndInsertAlbum(VaultContext context, AlbumModel result)
        {
            var album = TidalDaoMapper.MapTidalAlbumModelToDao(result);

            TidalDbInserter.UpsertAlbum(context, album);

            context.SaveChanges();

            return album;
        }

        private static TidalArtist MapAndInsertArtist(VaultContext context, ArtistModel result)
        {
            var artist = TidalDaoMapper.MapTidalArtistModelToDao(result);

            TidalDbInserter.InsertArtist(context, artist);

            context.SaveChanges();

            return artist;
        }

        private async Task<AlbumModel> GetAlbumAndMapAndInsert(VaultContext context, AlbumModel lesserAlbumModel)
        {
            var existingRecord = context.TidalAlbums.FirstOrDefault(p => p.Id == lesserAlbumModel.Id);
            if (existingRecord != null)
            {
                Log.Info($"Record exists: album {existingRecord.Id} {lesserAlbumModel.Title}");
                if (existingRecord.Upc != null)
                {
                    Log.Info("    album has UPC - will not get album or insert album or album artists");
                    return null;
                }
            }

            var albumResult = await _tidalIntegrator.GetAlbum(lesserAlbumModel.Id);
            if (albumResult == null)
            {
                Log.Warn($"Could not get album {lesserAlbumModel.Id} {lesserAlbumModel.Title} - inserting lesser album model");
                Console.WriteLine($"WARN Could not get album {lesserAlbumModel.Id} {lesserAlbumModel.Title} - inserting lesser album model");
                albumResult = lesserAlbumModel;
            }

            MapAndInsertAlbum(context, albumResult);

            return albumResult;
        }

        private static void MapAndInsertTrackArtists(VaultContext context, TrackModel track)
        {
            if (track?.Artist != null)
            {
                var trackArtist = TidalDaoMapper.MapTidalTrackArtistDao(track, track.Artist);
                TidalDbInserter.InsertTrackArtist(context, trackArtist);
            }

            if (track?.Artists != null)
            {
                var trackArtists = track.Artists.Select(i => TidalDaoMapper.MapTidalTrackArtistDao(track, i));

                foreach (var trackArtist in trackArtists)
                    TidalDbInserter.InsertTrackArtist(context, trackArtist);
            }

            context.SaveChanges();
        }

        private static void MapAndInsertAlbumArtists(VaultContext context, AlbumModel album)
        {
            if (album?.Artist != null)
            {
                var albumArtist = TidalDaoMapper.MapTidalAlbumArtistDao(album, album.Artist);
                TidalDbInserter.InsertAlbumArtist(context, albumArtist);
            }

            if (album?.Artists != null)
            {
                var albumArtists = album.Artists.Select(i => TidalDaoMapper.MapTidalAlbumArtistDao(album, i));

                foreach (var albumArtist in albumArtists)
                    TidalDbInserter.InsertAlbumArtist(context, albumArtist);
            }

            context.SaveChanges();
        }

        private static void MapAndInsertPlaylistFavorites(VaultContext context, IEnumerable<JsonListItem<PlaylistModel>> jsonListItems)
        {
            var favs = jsonListItems.Select(TidalDaoMapper.MapTidalPlaylistFavDao);

            foreach (var fav in favs)
                TidalDbInserter.InsertFavPlaylist(context, fav);

            context.SaveChanges();
        }

        private static void MapAndInsertAlbumFavorites(VaultContext context, IEnumerable<JsonListItem<AlbumModel>> jsonListItems)
        {
            var favs = jsonListItems.Select(TidalDaoMapper.MapTidalAlbumFavDao);

            foreach (var fav in favs)
                TidalDbInserter.InsertFavAlbum(context, fav);

            context.SaveChanges();
        }

        private static void MapAndInsertTrackFavorites(VaultContext context, IEnumerable<JsonListItem<TrackModel>> jsonListItems)
        {
            var favs = jsonListItems.Select(TidalDaoMapper.MapTidalTrackFavDao);

            foreach (var fav in favs)
                TidalDbInserter.InsertFavTrack(context, fav);

            context.SaveChanges();
        }

        private static void MapAndInsertArtistFavorites(VaultContext context, IEnumerable<JsonListItem<ArtistModel>> jsonListItems)
        {
            var favs = jsonListItems.Select(TidalDaoMapper.MapTidalArtistFavDao);

            foreach (var fav in favs)
                TidalDbInserter.InsertFavArtist(context, fav);

            context.SaveChanges();
        }
    }
}
