using System.Collections.Generic;
using System.Linq;
using Clockwork.Vault.Dao.Models.Tidal;
using Clockwork.Vault.Query.Tidal.ViewModels;
using Clockwork.Vault.Query.Tidal.ViewModels.Grouped;

namespace Clockwork.Vault.Query.Tidal
{
    public class TidalOrchestrator
    {
        private readonly TidalRepository _tidalRepository;

        public TidalOrchestrator()
        {
            _tidalRepository = new TidalRepository();
        }

        // All items - core entities

        public IList<TidalArtist> Artists => _tidalRepository.Artists;
        public IList<TidalAlbum> Albums => _tidalRepository.Albums;
        public IList<TidalPlaylist> Playlists => _tidalRepository.Playlists;
        public IList<TidalTrack> Tracks => _tidalRepository.Tracks;

        // All items - many-to-many

        public IList<TidalAlbumArtist> AlbumArtists => _tidalRepository.AlbumArtists;

        // Single item

        public TidalArtist GetArtist(int id) => _tidalRepository.GetArtist(id);
        public TidalAlbum GetAlbum(int id) => _tidalRepository.GetAlbum(id);
        public TidalPlaylist GetPlaylist(string id) => _tidalRepository.GetPlaylist(id);
        public TidalTrack GetTrack(int id) => _tidalRepository.GetTrack(id);

        // Get items by other item

        public IList<TidalTrackArtist> GetArtists(TidalTrack track) => _tidalRepository.GetArtists(track);
        public IList<TidalAlbumArtist> GetArtists(TidalAlbum album) => _tidalRepository.GetArtists(album);
        public IList<TidalAlbumTrack> GetTracks(TidalAlbum album) => _tidalRepository.GetTracks(album);

        // Expanded

        public TidalTrackExpanded GetTrackExpanded(int id)
        {
            var track = _tidalRepository.GetTrack(id);
            if (track == null)
                return null;

            var trackArtists = _tidalRepository.GetArtists(track)
                .Select(a => a.Artist)
                .ToList();

            var album = _tidalRepository.GetAlbum(track);

            var playlists = _tidalRepository.GetPlaylists(track);

            return new TidalTrackExpanded
            {
                Track = track,
                Artists = trackArtists,
                Album = album,
                Playlists = playlists
            };
        }

        public TidalAlbumExpanded GetAlbumExpanded(int id)
        {
            var album = _tidalRepository.GetAlbum(id);
            if (album == null)
                return null;

            var albumArtists = _tidalRepository.GetArtists(album)
                .Select(a => a.Artist)
                .ToList();

            var tracks = _tidalRepository.GetTracks(album)
                .Select(t => t.TrackId)
                .Select(GetTrackExpanded)
                .ToList();

            return new TidalAlbumExpanded
            {
                Album = album,
                Artists = albumArtists,
                Tracks = tracks
            };
        }

        public TidalPlaylistExpanded GetPlaylistExpanded(string id)
        {
            var playlist = _tidalRepository.GetPlaylist(id);
            if (playlist == null)
                return null;

            var tracks = _tidalRepository.GetTracks(playlist)
                .Select(t => (t.Position, t.TrackId))
                .Select(tuple => (tuple.Item1, GetTrackExpanded(tuple.Item2)))
                .ToList();

            var creator = DeterminePlaylistCreator(playlist.Creator.Id);

            return new TidalPlaylistExpanded
            {
                Playlist = playlist,
                Tracks = tracks,
                Creator = creator
            };
        }

        public TidalArtistExpanded GetArtistExpanded(int id)
        {
            var artist = _tidalRepository.GetArtist(id);
            if (artist == null)
                return null;

            var albums = _tidalRepository.GetAlbums(artist)
                .OrderBy(a => a.ReleaseDate)
                .ToList();

            var artistTracks = _tidalRepository.GetTracks(artist);
            var albumTracks = albums.SelectMany(a => _tidalRepository.GetTracks(a));
            var albumTrackIds = albumTracks.Select(at => at.TrackId);
            var nonAlbumTracks = artistTracks.Where(t => !albumTrackIds.Contains(t.Id))
                .ToList();

            return new TidalArtistExpanded
            {
                Artist = artist,
                Albums = albums,
                NonAlbumTracks = nonAlbumTracks
            };
        }

        // Grouped

        public IList<TidalPlaylistsGrouped> GetPlaylistsGroupedByCreator()
        {
            return _tidalRepository.Playlists?.GroupBy(p => p.CreatorId)
                .Select(group => new TidalPlaylistsGrouped
                {
                    Creator = DeterminePlaylistCreator(group.Key),
                    Playlists = @group.ToList()
                })
                .ToList();
        }

        public IList<ItemsGroupedByIsFavorite<TidalArtist>> GetArtistsOrderedByNameGroupedByIsFavorite()
        {
            var artists = _tidalRepository.Artists.OrderBy(a => a.Name);
            var favoriteArtists = _tidalRepository.FavoriteArtists.Select(f => f.ArtistId);

            return GroupItemsByIsFavorite(artists, favoriteArtists).ToList();
        }

        public IList<ItemsGroupedByIsFavorite<TidalAlbum>> GetAlbumsOrderedByTitleGroupedByIsFavorite()
        {
            var albums = _tidalRepository.Albums.OrderBy(a => a.Title);
            var favoriteAlbums = _tidalRepository.FavoriteAlbums.Select(f => f.AlbumId);

            return GroupItemsByIsFavorite(albums, favoriteAlbums).ToList();
        }

        public IList<ItemsGroupedByIsFavorite<TidalTrack>> GetTracksOrderedByTitleGroupedByIsFavorite()
        {
            var tracks = _tidalRepository.Tracks.OrderBy(a => a.Title);
            var favoriteTracks = _tidalRepository.FavoriteTracks.Select(f => f.TrackId);

            return GroupItemsByIsFavorite(tracks, favoriteTracks).ToList();
        }

        // Private

        private static IEnumerable<ItemsGroupedByIsFavorite<T>> GroupItemsByIsFavorite<T>(
            IEnumerable<T> allItems, IEnumerable<int> idsOfFavoriteItems)
            where T : TidalIntIdBase
        {
            return allItems.GroupBy(a => idsOfFavoriteItems.Contains(a.Id))
                .Select(group => new ItemsGroupedByIsFavorite<T>
                {
                    IsFavorite = group.Key,
                    Items = group.ToList()
                })
                // Favorites first
                .Reverse();
        }

        private string DeterminePlaylistCreator(int creatorId)
        {
            if (creatorId == 0)
                return "Tidal";

            var found = _tidalRepository.PlaylistCreators.FirstOrDefault(c => c.CreatorId == creatorId);
            return found != null 
                ? found.Name 
                : creatorId.ToString();
        }
    }
}