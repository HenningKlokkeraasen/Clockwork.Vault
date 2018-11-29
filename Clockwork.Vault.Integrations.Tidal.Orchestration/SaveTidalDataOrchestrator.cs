﻿using System;
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
    public static class SaveTidalDataOrchestrator
    {
        private static readonly ILog Log = LogManager.GetLogger("Default");

        public static async Task SavePlaylists(OpenTidlSession session, VaultContext context, int limit = 9999)
        {
            var playlistsResult = await session.GetUserPlaylists(limit);
            await SavePlaylistsAndTracksAndAlbumsAndArtists(session, context, playlistsResult.Items);
        }

        public static async Task SaveUserFavPlaylists(OpenTidlSession session, VaultContext context, int limit = 9999)
        {
            var favsResult = await session.GetFavoritePlaylists(limit);
            var items = favsResult.Items.Select(i => i.Item).ToList();
            await SavePlaylistsAndTracksAndAlbumsAndArtists(session, context, items);
            MapAndInsertPlaylistFavorites(context, favsResult.Items);
        }

        public static async Task SaveUserFavTracks(OpenTidlSession session, VaultContext context, int limit = 9999)
        {
            var favsResult = await session.GetFavoriteTracks(limit);
            var items = favsResult.Items.Select(i => i.Item).ToList();

            var tracks = MapAndInsertTracks(context, items);

            var insertedAlbums = new List<AlbumModel>();
            var insertedArtists = new List<ArtistModel>();

            await GetAlbumsAndSaveAlbumsAndArtists(context, items, insertedAlbums, insertedArtists);
            MapAndInsertTrackFavorites(context, favsResult.Items);
        }

        public static async Task SaveUserFavAlbums(OpenTidlSession session, VaultContext context, int limit = 9999)
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

        public static async Task SaveUserFavArtists(OpenTidlSession session, VaultContext context, int limit = 9999)
        {
            var favsResult = await session.GetFavoriteArtists(limit);
            var items = favsResult.Items.Select(i => i.Item).ToList();

            foreach (var item in items)
                MapAndInsertArtist(context, item);

            MapAndInsertArtistFavorites(context, favsResult.Items);
        }

        private static async Task SaveAlbumTracks(VaultContext context, AlbumModel item)
        {
            var tracks = await TidalIntegrator.GetAlbumTracks(item.Id);
            if (tracks == null)
            {
                Console.WriteLine($"Could not get album tracks for album {item.Title} ({item.Id})");
                Log.Error($"Could not get album tracks for album {item.Title} ({item.Id})");
                return;
            }

            MapAndInsertTracks(context, tracks.Items);
            MapAndInsertAlbumTracks(context, tracks.Items, item);
        }

        public static async Task EnsureAlbumUpc(VaultContext context, IterationSettings iterationSettings)
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
                var albumResult = await TidalIntegrator.GetAlbum(tidalAlbum.Id);

                if (albumResult == null)
                {
                    Log.Warn($"Could not get album {tidalAlbum.Id} {tidalAlbum.Title}");
                }
                else
                {
                    var album = DaoMapper.MapTidalAlbumModelToDao(albumResult);
                    DbInserter.UpdateFields(context, album, tidalAlbum);
                }

                Log.Info($"Sleeping for {sleepTimeInSeconds} seconds");
                Thread.Sleep(sleepTimeInSeconds * 1000);
            }
        }

        public static async Task EnsureTrackIsrc(VaultContext context, IterationSettings iterationSettings)
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
                var trackResult = await TidalIntegrator.GetTrack(tidalTrack.Id);

                if (trackResult == null)
                {
                    Log.Warn($"Could not get track {tidalTrack.Id} {tidalTrack.Title}");
                }
                else
                {
                    var album = DaoMapper.MapTidalTrackModelToDao(trackResult);
                    DbInserter.UpdateFields(context, album, tidalTrack);
                }

                Log.Info($"Sleeping for {sleepTimeInSeconds} seconds");
                Thread.Sleep(sleepTimeInSeconds * 1000);
            }
        }

        private static async Task SavePlaylistsAndTracksAndAlbumsAndArtists(OpenTidlSession session, VaultContext context, IList<PlaylistModel> playlistsItems)
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

        private static async Task GetAlbumsAndSaveAlbumsAndArtists(VaultContext context, IEnumerable<TrackModel> tracksItems,
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

                foreach (var trackArtist in track.Artists)
                    SaveArtist(context, insertedArtists, trackArtist);

                MapAndInsertTrackArtists(context, track);
            }
        }

        private static async Task<AlbumModel> GetAlbumAndSaveAlbumAndArtists(VaultContext context,
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

            if (albumResult.Artists == null || !albumResult.Artists.Any())
            {
                //TODO
            }
            foreach (var albumArtist in albumResult.Artists ?? Enumerable.Empty<ArtistModel>())
                SaveArtist(context, insertedArtists, albumArtist);

            MapAndInsertAlbumArtists(context, albumResult);
            return albumResult;
        }

        private static void SaveAlbumAndArtists(VaultContext context, ICollection<ArtistModel> insertedArtists, AlbumModel item)
        {
            MapAndInsertAlbum(context, item);

            if (item.Artists == null || !item.Artists.Any())
            {
                //TODO
            }
            foreach (var albumArtist in item.Artists ?? Enumerable.Empty<ArtistModel>())
                SaveArtist(context, insertedArtists, albumArtist);

            MapAndInsertAlbumArtists(context, item);
        }
        
        private static void SaveArtist(VaultContext context, ICollection<ArtistModel> insertedArtists, ArtistModel item)
        {
            if (insertedArtists.All(a => a.Id != item.Id))
            {
                MapAndInsertArtist(context, item);
                insertedArtists.Add(item);
            }
        }

        private static void MapAndInsertCreators(VaultContext context, IEnumerable<PlaylistModel> result)
        {
            var creators = result.Select(DaoMapper.MapTidalCreatorModelToDao);

            var distinctCreators = creators
                .GroupBy(c => c.Id)
                .Select(group => group.First())
                .ToList();

            distinctCreators.ForEach(c => DbInserter.InsertCreator(context, c));

            context.SaveChanges();
        }

        private static IEnumerable<TidalPlaylist> MapAndInsertPlaylists(VaultContext context, IEnumerable<PlaylistModel> result)
        {
            var playlists = result.Select(DaoMapper.MapTidalPlaylistModelToDao).ToList();

            playlists.ForEach(p => DbInserter.InsertPlaylist(context, p));

            context.SaveChanges();

            return playlists;
        }

        private static IEnumerable<TidalTrack> MapAndInsertTracks(VaultContext context, IEnumerable<TrackModel> result)
        {
            var tracks = result.Select(DaoMapper.MapTidalTrackModelToDao).ToList();

            var trackGroups = tracks.GroupBy(t => t.Id).ToList();

            trackGroups.Where(group => group.Count() > 1)
                .ToList()
                .ForEach(group => Log.Warn($"Duplicate track {group.Key} {group.First().Title}"));

            var distinctTracks = trackGroups
                .Select(group => group.First())
                .ToList();

            distinctTracks.ForEach(t => DbInserter.UpsertTrack(context, t));

            context.SaveChanges();

            return distinctTracks;
        }

        private static void MapAndInsertAlbumTracks(VaultContext context, IEnumerable<TrackModel> tracks, AlbumModel album)
        {
            var position = 1;
            var albumTracks = tracks.Select(i => DaoMapper.MapTidalAlbumTrackDao(i.Id, album.Id, position++))
                .ToList();

            albumTracks.ForEach(pt => DbInserter.InsertAlbumTrack(context, pt));

            context.SaveChanges();
        }

        private static void MapAndInsertPlaylistTracks(VaultContext context, IEnumerable<TidalTrack> tracks, TidalPlaylist playlist)
        {
            var position = 1;
            var playlistTracks = tracks.Select(i => DaoMapper.MapTidalPlaylistTrackDao(i.Id, playlist.Uuid, position++))
                .ToList();

            playlistTracks.ForEach(pt => DbInserter.InsertPlaylistTrack(context, pt));

            context.SaveChanges();
        }

        private static TidalAlbum MapAndInsertAlbum(VaultContext context, AlbumModel result)
        {
            var album = DaoMapper.MapTidalAlbumModelToDao(result);

            DbInserter.UpsertAlbum(context, album);

            context.SaveChanges();

            return album;
        }

        private static TidalArtist MapAndInsertArtist(VaultContext context, ArtistModel result)
        {
            var artist = DaoMapper.MapTidalArtistModelToDao(result);

            DbInserter.InsertArtist(context, artist);

            context.SaveChanges();

            return artist;
        }

        private static async Task<AlbumModel> GetAlbumAndMapAndInsert(VaultContext context, AlbumModel lesserAlbumModel)
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

            var albumResult = await TidalIntegrator.GetAlbum(lesserAlbumModel.Id);
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
            if (track?.Artists == null)
                return;

            var trackArtists = track.Artists.Select(i => DaoMapper.MapTidalTrackArtistDao(track, i));

            foreach (var trackArtist in trackArtists)
                DbInserter.InsertTrackArtist(context, trackArtist);

            context.SaveChanges();
        }

        private static void MapAndInsertAlbumArtists(VaultContext context, AlbumModel album)
        {
            if (album?.Artists == null)
                return;

            var albumArtists = album.Artists.Select(i => DaoMapper.MapTidalAlbumArtistDao(album, i));

            foreach (var albumArtist in albumArtists)
                DbInserter.InsertAlbumArtist(context, albumArtist);

            context.SaveChanges();
        }

        private static void MapAndInsertPlaylistFavorites(VaultContext context, IEnumerable<JsonListItem<PlaylistModel>> jsonListItems)
        {
            var favs = jsonListItems.Select(DaoMapper.MapTidalPlaylistFavDao);

            foreach (var fav in favs)
                DbInserter.InsertFavPlaylist(context, fav);

            context.SaveChanges();
        }

        private static void MapAndInsertAlbumFavorites(VaultContext context, IEnumerable<JsonListItem<AlbumModel>> jsonListItems)
        {
            var favs = jsonListItems.Select(DaoMapper.MapTidalAlbumFavDao);

            foreach (var fav in favs)
                DbInserter.InsertFavAlbum(context, fav);

            context.SaveChanges();
        }

        private static void MapAndInsertTrackFavorites(VaultContext context, IEnumerable<JsonListItem<TrackModel>> jsonListItems)
        {
            var favs = jsonListItems.Select(DaoMapper.MapTidalTrackFavDao);

            foreach (var fav in favs)
                DbInserter.InsertFavTrack(context, fav);

            context.SaveChanges();
        }

        private static void MapAndInsertArtistFavorites(VaultContext context, IEnumerable<JsonListItem<ArtistModel>> jsonListItems)
        {
            var favs = jsonListItems.Select(DaoMapper.MapTidalArtistFavDao);

            foreach (var fav in favs)
                DbInserter.InsertFavArtist(context, fav);

            context.SaveChanges();
        }
    }
}
