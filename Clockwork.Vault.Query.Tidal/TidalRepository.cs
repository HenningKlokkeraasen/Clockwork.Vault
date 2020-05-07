using System.Collections.Generic;
using System.Linq;
using Clockwork.Vault.Dao;
using Clockwork.Vault.Dao.Models.Tidal;
using Clockwork.Vault.Dao.Models.Tidal.ManualData;
using Clockwork.Vault.Integrations.Tidal.Orchestration;

namespace Clockwork.Vault.Query.Tidal
{
    internal class TidalRepository
    {
        private readonly VaultContext _vaultContext;

        internal TidalRepository()
        {
            _vaultContext = new VaultContext();
        }

        // All items - core entities
        internal IList<TidalArtist> Artists => _vaultContext.TidalArtists.ProjectToList();
        internal IList<TidalAlbum> Albums => _vaultContext.TidalAlbums.ProjectToList();
        internal IList<TidalPlaylist> Playlists => _vaultContext.TidalPlaylists.ProjectToList();
        internal IList<TidalTrack> Tracks => _vaultContext.TidalTracks.ProjectToList();

        // All items - many-to-many entities
        internal IList<TidalAlbumArtist> AlbumArtists => _vaultContext.TidalAlbumArtists.ProjectToList();

        // Item by ID
        internal TidalArtist GetArtist(int id) => _vaultContext.TidalArtists.FirstOrDefault(a => a.Id == id);
        internal TidalAlbum GetAlbum(int id) => _vaultContext.TidalAlbums.FirstOrDefault(a => a.Id == id);
        internal TidalPlaylist GetPlaylist(string id) => _vaultContext.TidalPlaylists.FirstOrDefault(a => a.Uuid == id);
        internal TidalTrack GetTrack(int id) => _vaultContext.TidalTracks.FirstOrDefault(a => a.Id == id);

        // Items by other Item

        internal IList<TidalTrackArtist> GetArtists(TidalTrack track) =>
            _vaultContext.TidalTrackArtists.Where(t => t.TrackId == track.Id).ProjectToList();

        internal IList<TidalAlbumArtist> GetArtists(TidalAlbum album) =>
            _vaultContext.TidalAlbumArtists.Where(t => t.AlbumId == album.Id).ProjectToList();

        internal IList<TidalAlbumTrack> GetTracks(TidalAlbum album) =>
            _vaultContext.TidalAlbumTracks.Where(at => at.AlbumId == album.Id).ProjectToList();

        internal IList<TidalPlaylistTrack> GetTracks(TidalPlaylist playlist) =>
            _vaultContext.TidalPlaylistTracks.Where(at => at.PlaylistId == playlist.Uuid).ProjectToList();

        internal IList<TidalAlbum> GetReleases(TidalArtist artist)
        {
            var albumArtists = _vaultContext.TidalAlbumArtists.Where(aa => aa.ArtistId == artist.Id);
            var albumIds = albumArtists.Select(a => a.AlbumId).Distinct();
            return _vaultContext.TidalAlbums.Where(a => albumIds.Contains(a.Id)).ProjectToList();
        }

        internal IList<TidalTrack> GetTracks(TidalArtist artist) =>
            _vaultContext.TidalTrackArtists.Where(ta => ta.ArtistId == artist.Id)
                .Select(ta => _vaultContext.TidalTracks.FirstOrDefault(t => t.Id == ta.TrackId))
                .ToList();

        public IList<TidalPlaylist> GetPlaylists(TidalTrack track)
        {
            return _vaultContext.TidalPlaylistTracks.Where(pt => pt.TrackId == track.Id)
                .Select(pt => pt.PlaylistId)
                .Select(id => _vaultContext.TidalPlaylists.FirstOrDefault(p => p.Uuid == id))
                .ProjectToList();
        }

        // Item by other Item

        internal TidalAlbum GetAlbum(TidalTrack track) =>
            _vaultContext.TidalAlbumTracks.FirstOrDefault(t => t.TrackId == track.Id)?.Album;

        // Favorites
        internal IList<TidalUserFavoriteAlbum> FavoriteAlbums => _vaultContext.TidalFavoriteAlbums.ProjectToList();
        internal IList<TidalUserFavoriteArtist> FavoriteArtists => _vaultContext.TidalFavoriteArtists.ProjectToList();
        internal IList<TidalUserFavoriteTrack> FavoriteTracks => _vaultContext.TidalFavoriteTracks.ProjectToList();
        internal IList<TidalUserFavoritePlaylist> FavoritePlaylists => _vaultContext.TidalFavoritePlaylists.ProjectToList();

        // Manual data
        internal IList<TidalPlaylistCreatorManual> PlaylistCreators => _vaultContext.TidalPlaylistCreators.ProjectToList();
    }
}