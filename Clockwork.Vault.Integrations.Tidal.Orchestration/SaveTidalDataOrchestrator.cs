using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Clockwork.Vault.Integrations.Tidal.Dao;
using Clockwork.Vault.Integrations.Tidal.Dao.Models;
using log4net;
using OpenTidl.Methods;
using OpenTidl.Models;
using OpenTidl.Models.Base;

namespace Clockwork.Vault.Integrations.Tidal.Orchestration
{
    public static class SaveTidalDataOrchestrator
    {
        private static readonly ILog Log = LogManager.GetLogger("Default");

        public static async Task SavePlaylists(OpenTidlSession session, VaultContext context)
        {
            var playlistsResult = await session.GetUserPlaylists(5);//TODO
            await SavePlaylists(session, context, playlistsResult.Items);
        }

        public static async Task SaveUserFavPlaylists(OpenTidlSession session, VaultContext context)
        {
            var favsResult = await session.GetFavoritePlaylists(5);//TODO
            var items = favsResult.Items.Select(i => i.Item).ToList();
            await SavePlaylists(session, context, items);
            MapAndInsertPlaylistFavorites(context, favsResult.Items);
        }

        public static async Task SaveUserFavTracks(OpenTidlSession session, VaultContext context)
        {
            var favsResult = await session.GetFavoriteTracks(5);//TODO
            var items = favsResult.Items.Select(i => i.Item).ToList();

            var tracks = MapAndInsertTracks(context, items);

            var insertedAlbums = new List<AlbumModel>();
            var insertedArtists = new List<ArtistModel>();

            await SaveAlbumsAndArtists(context, items, insertedAlbums, insertedArtists);
            MapAndInsertTrackFavorites(context, favsResult.Items);
        }

        public static async Task SaveUserFavAlbums(OpenTidlSession session, VaultContext context)
        {
            var favsResult = await session.GetFavoriteAlbums(5);//TODO
            var items = favsResult.Items.Select(i => i.Item).ToList();

            var insertedArtists = new List<ArtistModel>();

            foreach (var item in items)
            {
                var albumResult = await GetAlbumAndMapAndInsert(context, item);

                foreach (var albumArtist in albumResult.Artists ?? Enumerable.Empty<ArtistModel>())
                {
                    await SaveArtist(context, insertedArtists, albumArtist);
                }

                MapAndInsertAlbumArtists(context, albumResult);
            }

            MapAndInsertAlbumFavorites(context, favsResult.Items);
        }

        private static async Task SavePlaylists(OpenTidlSession session, VaultContext context, IList<PlaylistModel> playlistsItems)
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
                await SaveAlbumsAndArtists(context, tracksResult.Items, insertedAlbums, insertedArtists);
            }
        }

        private static async Task SaveAlbumsAndArtists(VaultContext context, IEnumerable<TrackModel> tracksItems,
             ICollection<AlbumModel> insertedAlbums, ICollection<ArtistModel> insertedArtists)
        {
            foreach (var track in tracksItems)
            {
                if (insertedAlbums.All(a => a.Id != track.Album.Id))
                {
                    var albumResult = await GetAlbumAndMapAndInsert(context, track.Album);
                    insertedAlbums.Add(albumResult);

                    foreach (var albumArtist in albumResult.Artists ?? Enumerable.Empty<ArtistModel>())
                    {
                        await SaveArtist(context, insertedArtists, albumArtist);
                    }

                    MapAndInsertAlbumArtists(context, albumResult);
                }

                foreach (var trackArtist in track.Artists)
                {
                    await SaveArtist(context, insertedArtists, trackArtist);
                }

                MapAndInsertTrackArtists(context, track);
            }
        }

        private static async Task SaveArtist(VaultContext context, ICollection<ArtistModel> insertedArtists, ArtistModel x)
        {
            if (insertedArtists.All(a => a.Id != x.Id))
            {
                var artistResult = await GetArtistAndMapAndInsert(context, x);
                insertedArtists.Add(artistResult);
            }
        }

        private static void MapAndInsertCreators(VaultContext context, IEnumerable<PlaylistModel> result)
        {
            var creators = result.Select(DaoMapper.MapTidalCreatorModelToDao);

            var distinctCreators = creators
                .GroupBy(c => c.Id)
                .Select(group => group.First());

            foreach (var creator in distinctCreators)
                DbInserter.InsertCreator(context, creator);

            context.SaveChanges();
        }

        private static IEnumerable<TidalPlaylist> MapAndInsertPlaylists(VaultContext context, IEnumerable<PlaylistModel> result)
        {
            var playlists = result.Select(DaoMapper.MapTidalPlaylistModelToDao);

            foreach (var playlist in playlists)
                DbInserter.InsertPlaylist(context, playlist);

            context.SaveChanges();

            return playlists;
        }

        private static IEnumerable<TidalTrack> MapAndInsertTracks(VaultContext context, IEnumerable<TrackModel> result)
        {
            var tracks = result.Select(DaoMapper.MapTidalTrackModelToDao);

            foreach (var track in tracks)
                DbInserter.InsertTrack(context, track);

            context.SaveChanges();

            return tracks;
        }

        private static void MapAndInsertPlaylistTracks(VaultContext context, IEnumerable<TidalTrack> tracks, TidalPlaylist playlist)
        {
            var position = 1;
            var playlistTracks = tracks.Select(i => DaoMapper.MapTidalPlaylistTrackDao(i, playlist, position++));

            foreach (var playlistTrack in playlistTracks)
                DbInserter.InsertPlaylistTrack(context, playlistTrack);

            context.SaveChanges();
        }

        private static TidalAlbum MapAndInsertAlbum(VaultContext context, AlbumModel result)
        {
            var album = DaoMapper.MapTidalAlbumModelToDao(result);

            DbInserter.InsertAlbum(context, album);

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

        private static async Task<ArtistModel> GetArtistAndMapAndInsert(VaultContext context, ArtistModel trackArtist)
        {
            var artistResult = await TidalIntegrator.GetArtist(trackArtist.Id);
            if (artistResult == null)
            {
                Log.Warn($"WARN Could not get artist {trackArtist.Id} {trackArtist.Name} - inserting lesser artist model");
                Console.WriteLine($"WARN Could not get artist {trackArtist.Id} {trackArtist.Name} - inserting lesser artist model");
                artistResult = trackArtist;
            }

            MapAndInsertArtist(context, artistResult);

            return artistResult;
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
