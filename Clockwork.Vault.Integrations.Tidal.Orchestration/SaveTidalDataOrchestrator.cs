using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Clockwork.Vault.Integrations.Tidal.Dao;
using Clockwork.Vault.Integrations.Tidal.Dao.Models;
using OpenTidl.Methods;
using OpenTidl.Models;

namespace Clockwork.Vault.Integrations.Tidal.Orchestration
{
    public static class SaveTidalDataOrchestrator
    {
        public static async Task SavePlaylists(OpenTidlSession session, VaultContext context)
        {
            var playlistsResult = await session.GetUserPlaylists(5);

            MapAndInsertCreators(context, playlistsResult.Items);

            var playlists = MapAndInsertPlaylists(context, playlistsResult.Items);

            var insertedAlbums = new List<AlbumModel>();

            foreach (var playlist in playlists)
            {
                var tracksResult = await session.GetPlaylistTracks(playlist.Uuid);

                var tracks = MapAndInsertTracks(context, tracksResult.Items);

                MapAndInsertPlaylistTracks(context, tracks, playlist);

                foreach (var track in tracksResult.Items)
                {
                    if (insertedAlbums.All(a => a.Id != track.Album.Id))
                    {
                        var albumResult = await TidalIntegrator.GetAlbum(track.Album.Id);
                        if (albumResult == null)
                        {
                            Console.WriteLine($"WARN Could not get album {track.Album.Id} {track.Album.Title} - inserting lesser album model");
                            albumResult = track.Album;
                        }
                        MapAndInsertAlbum(context, albumResult);
                        insertedAlbums.Add(albumResult);

                        // TODO insert artists
                        // TODO insert albumArtists
                    }
                    // TODO insert artists
                    // TODO insert trackArtists
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
    }
}
