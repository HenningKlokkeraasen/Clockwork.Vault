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
            var playlistsFavResult = await session.GetFavoritePlaylists(5);//TODO
            var playlistsResult = playlistsFavResult.Items.Select(i => i.Item).ToList();
            await SavePlaylists(session, context, playlistsResult);
            MapAndInsertPlaylistFavorites(context, playlistsFavResult.Items);
        }

        private static async Task SavePlaylists(OpenTidlSession session, VaultContext context, IList<PlaylistModel> playlistsResult)
        {
            MapAndInsertCreators(context, playlistsResult);

            var playlists = MapAndInsertPlaylists(context, playlistsResult);

            var insertedAlbums = new List<AlbumModel>();
            var insertedArtists = new List<ArtistModel>();

            foreach (var playlist in playlists)
            {
                var tracksResult = await session.GetPlaylistTracks(playlist.Uuid);

                var tracks = MapAndInsertTracks(context, tracksResult.Items);

                MapAndInsertPlaylistTracks(context, tracks, playlist);

                foreach (var track in tracksResult.Items)
                {
                    if (insertedAlbums.All(a => a.Id != track.Album.Id))
                    {
                        var albumResult = await GetAlbumAndMapAndInsert(context, track);
                        insertedAlbums.Add(albumResult);

                        foreach (var albumArtist in albumResult.Artists ?? Enumerable.Empty<ArtistModel>())
                        {
                            if (insertedArtists.All(a => a.Id != albumArtist.Id))
                            {
                                var artistResult = await GetArtistAndMapAndInsert(context, albumArtist);
                                insertedArtists.Add(artistResult);
                            }
                        }

                        MapAndInsertAlbumArtists(context, albumResult);
                    }

                    foreach (var trackArtist in track.Artists)
                    {
                        if (insertedArtists.All(a => a.Id != trackArtist.Id))
                        {
                            var artistResult = await GetArtistAndMapAndInsert(context, trackArtist);
                            insertedArtists.Add(artistResult);
                        }
                    }

                    MapAndInsertTrackArtists(context, track);
                }
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
            var playlistTracks = tracks.Select(i => DaoMapper.MapTidalPlaylistTrackDao(i, playlist));

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

        private static async Task<AlbumModel> GetAlbumAndMapAndInsert(VaultContext context, TrackModel track)
        {
            var albumResult = await TidalIntegrator.GetAlbum(track.Album.Id);
            if (albumResult == null)
            {
                Log.Warn($"Could not get album {track.Album.Id} {track.Album.Title} - inserting lesser album model");
                Console.WriteLine($"WARN Could not get album {track.Album.Id} {track.Album.Title} - inserting lesser album model");
                albumResult = track.Album;
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
