using System.Collections.Generic;
using System.Linq;
using Clockwork.Vault.Dao;
using Clockwork.Vault.Dao.Models.Tidal;
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

        internal IList<TidalArtist> Artists => _vaultContext.TidalArtists.ProjectToList();

        internal IList<TidalAlbum> Albums => _vaultContext.TidalAlbums.ProjectToList();

        internal IList<TidalPlaylist> Playlists => _vaultContext.TidalPlaylists.ProjectToList();

        internal IList<TidalTrack> Tracks => _vaultContext.TidalTracks.ProjectToList();

        internal IList<TidalAlbumArtist> AlbumArtists => _vaultContext.TidalAlbumArtists.ProjectToList();

        internal TidalArtist GetArtist(int id) => _vaultContext.TidalArtists.FirstOrDefault(a => a.Id == id);

        internal TidalAlbum GetAlbum(int id) => _vaultContext.TidalAlbums.FirstOrDefault(a => a.Id == id);

        internal TidalPlaylist GetPlaylist(string id) => _vaultContext.TidalPlaylists.FirstOrDefault(a => a.Uuid == id);

        internal TidalTrack GetTrack(int id) => _vaultContext.TidalTracks.FirstOrDefault(a => a.Id == id);

        internal IList<TidalTrackArtist> GetArtists(TidalTrack track) =>
            _vaultContext.TidalTrackArtists.Where(t => t.TrackId == track.Id).ProjectToList();

        internal IList<TidalAlbumArtist> GetArtists(TidalAlbum album) =>
            _vaultContext.TidalAlbumArtists.Where(t => t.AlbumId == album.Id).ProjectToList();

        internal IList<TidalAlbumTrack> GetTracks(TidalAlbum album) =>
            _vaultContext.TidalAlbumTracks.Where(at => at.AlbumId == album.Id).ProjectToList();

        internal IList<TidalPlaylistTrack> GetTracks(TidalPlaylist playlist) =>
            _vaultContext.TidalPlaylistTracks.Where(at => at.PlaylistId == playlist.Uuid).ProjectToList();
    }
}