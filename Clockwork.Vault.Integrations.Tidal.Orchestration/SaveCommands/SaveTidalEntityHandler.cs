using System.Collections.Generic;
using System.Linq;
using Clockwork.Vault.Dao;
using Clockwork.Vault.Dao.Models.Tidal;
using OpenTidl.Models;
using OpenTidl.Models.Base;

namespace Clockwork.Vault.Integrations.Tidal.Orchestration.SaveCommands
{
    internal class SaveTidalEntityHandler
    {
        private readonly VaultContext _vaultContext;

        internal SaveTidalEntityHandler(VaultContext vaultContext)
        {
            _vaultContext = vaultContext;
        }

        // Core entities

        internal TidalArtist MapAndInsertArtist(ArtistModel tidalArtist)
        {
            var artist = TidalDaoMapper.MapTidalArtistModelToDao(tidalArtist);
            TidalDbInserter.InsertArtist(_vaultContext, artist);
            return SaveChangesToDbAndReturnDao(artist);
        }

        internal TidalAlbum MapAndUpsertAlbum(AlbumModel tidalAlbum)
        {
            var album = TidalDaoMapper.MapTidalAlbumModelToDao(tidalAlbum);
            TidalDbInserter.UpsertAlbum(_vaultContext, album);
            return SaveChangesToDbAndReturnDao(album);
        }

        internal IEnumerable<TidalPlaylist> MapAndInsertPlaylists(IEnumerable<PlaylistModel> tidalPlaylists)
        {
            var playlists = tidalPlaylists.Select(TidalDaoMapper.MapTidalPlaylistModelToDao).ToList();
            playlists.ForEach(p => TidalDbInserter.InsertPlaylist(_vaultContext, p));
            return SaveChangesToDbAndReturnDao(playlists);
        }

        internal (IEnumerable<TidalTrack>, IEnumerable<string>) MapAndUpsertTracks(IEnumerable<TrackModel> tidalTracks)
        {
            var log = new List<string>();

            var tracks = tidalTracks.Select(TidalDaoMapper.MapTidalTrackModelToDao).ToList();

            var trackGroups = tracks.GroupBy(t => t.Id).ToList();

            trackGroups.Where(group => group.Count() > 1)
                .ToList()
                .ForEach(group => log.Add($"WARN Duplicate track {group.Key} {group.First().Title}"));

            var distinctTracks = trackGroups
                .Select(group => group.First())
                .ToList();

            distinctTracks.ForEach(t => TidalDbInserter.UpsertTrack(_vaultContext, t));

            return SaveChangesToDbAndReturnDao((distinctTracks, log));
        }

        // Secondary entities

        internal IList<TidalCreator> MapAndInsertCreators(IEnumerable<PlaylistModel> tidalPlaylists)
        {
            var creators = tidalPlaylists.Select(TidalDaoMapper.MapTidalCreatorModelToDao);

            var distinctCreators = creators
                .GroupBy(c => c.Id)
                .Select(group => group.First())
                .ToList();

            distinctCreators.ForEach(c => TidalDbInserter.InsertCreator(_vaultContext, c));

            return SaveChangesToDbAndReturnDao(distinctCreators);
        }

        // Many-to-many entities

        internal void MapAndInsertAlbumTracks(IEnumerable<TrackModel> tidalTracks, AlbumModel tidalAlbum)
        {
            var position = 1;
            var albumTracks = tidalTracks.Select(i => TidalDaoMapper.MapTidalAlbumTrackDao(i.Id, tidalAlbum.Id, position++))
                .ToList();

            albumTracks.ForEach(pt => TidalDbInserter.InsertAlbumTrack(_vaultContext, pt));

            _vaultContext.SaveChanges();
        }

        internal void MapAndInsertPlaylistTracks(IEnumerable<TidalTrack> tidalTracks, TidalPlaylist tidalPlaylist)
        {
            var position = 1;
            var playlistTracks = tidalTracks.Select(i => TidalDaoMapper.MapTidalPlaylistTrackDao(i.Id, tidalPlaylist.Uuid, position++))
                .ToList();

            playlistTracks.ForEach(pt => TidalDbInserter.InsertPlaylistTrack(_vaultContext, pt));

            _vaultContext.SaveChanges();
        }

        // Favorites

        internal void MapAndInsertPlaylistFavorites(IEnumerable<JsonListItem<PlaylistModel>> jsonListItems)
        {
            var favs = jsonListItems.Select(TidalDaoMapper.MapTidalPlaylistFavDao);

            foreach (var fav in favs)
                TidalDbInserter.InsertFavPlaylist(_vaultContext, fav);

            _vaultContext.SaveChanges();
        }

        internal void MapAndInsertAlbumFavorites(IEnumerable<JsonListItem<AlbumModel>> jsonListItems)
        {
            var favs = jsonListItems.Select(TidalDaoMapper.MapTidalAlbumFavDao);

            foreach (var fav in favs)
                TidalDbInserter.InsertFavAlbum(_vaultContext, fav);

            _vaultContext.SaveChanges();
        }

        internal void MapAndInsertTrackFavorites(IEnumerable<JsonListItem<TrackModel>> jsonListItems)
        {
            var favs = jsonListItems.Select(TidalDaoMapper.MapTidalTrackFavDao);

            foreach (var fav in favs)
                TidalDbInserter.InsertFavTrack(_vaultContext, fav);

            _vaultContext.SaveChanges();
        }

        internal void MapAndInsertArtistFavorites(IEnumerable<JsonListItem<ArtistModel>> jsonListItems)
        {
            var favs = jsonListItems.Select(TidalDaoMapper.MapTidalArtistFavDao);

            foreach (var fav in favs)
                TidalDbInserter.InsertFavArtist(_vaultContext, fav);

            _vaultContext.SaveChanges();
        }

        // Private

        private T SaveChangesToDbAndReturnDao<T>(T dao)
        {
            _vaultContext.SaveChanges();
            return dao;
        }
    }
}