using System.Collections.Generic;
using System.Linq;
using Clockwork.Vault.Dao;
using Clockwork.Vault.Dao.Models.Tidal;

namespace Clockwork.Vault.Integrations.Tidal.Orchestration
{
    internal class TidalRepository
    {
        private readonly VaultContext _vaultContext;

        internal TidalRepository()
        {
            _vaultContext = new VaultContext();
        }

        internal IList<TidalArtist> Artists => _vaultContext.Artists.ProjectToList();

        internal IList<TidalAlbum> Albums => _vaultContext.Albums.ProjectToList();

        internal IList<TidalPlaylist> Playlists => _vaultContext.Playlists.ProjectToList();

        internal IList<TidalTrack> Tracks => _vaultContext.Tracks.ProjectToList();

        internal TidalArtist GetArtist(int id) => _vaultContext.Artists.FirstOrDefault(a => a.Id == id);

        internal TidalAlbum GetAlbum(int id) => _vaultContext.Albums.FirstOrDefault(a => a.Id == id);

        internal TidalPlaylist GetPlaylist(string id) => _vaultContext.Playlists.FirstOrDefault(a => a.Uuid == id);

        internal TidalTrack GetTrack(int id) => _vaultContext.Tracks.FirstOrDefault(a => a.Id == id);

        internal IList<TidalTrackArtist> GetArtists(TidalTrack track) =>
            _vaultContext.TrackArtists.Where(t => t.TrackId == track.Id).ProjectToList();

        internal IList<TidalAlbumArtist> GetArtists(TidalAlbum album) =>
            _vaultContext.AlbumArtists.Where(t => t.AlbumId == album.Id).ProjectToList();

        internal IList<TidalAlbumTrack> GetTracks(TidalAlbum album) =>
            _vaultContext.AlbumTracks.Where(at => at.AlbumId == album.Id).ProjectToList();

        internal IList<TidalPlaylistTrack> GetTracks(TidalPlaylist playlist) =>
            _vaultContext.PlaylistTracks.Where(at => at.PlaylistId == playlist.Uuid).ProjectToList();
    }
}