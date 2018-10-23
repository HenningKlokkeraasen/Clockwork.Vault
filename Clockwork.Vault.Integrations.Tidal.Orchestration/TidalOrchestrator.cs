using System.Collections.Generic;
using System.Linq;
using Clockwork.Vault.Integrations.Tidal.Dao;
using Clockwork.Vault.Integrations.Tidal.Dao.Models;

namespace Clockwork.Vault.Integrations.Tidal.Orchestration
{
    public class TidalOrchestrator
    {
        private readonly VaultContext _vaultContext;

        public TidalOrchestrator()
        {
            _vaultContext = new VaultContext();
        }

        public IList<TidalArtist> Artists => _vaultContext.Artists.ProjectToList();

        public IList<TidalAlbum> Albums => _vaultContext.Albums.ProjectToList();

        public IList<TidalPlaylist> Playlists => _vaultContext.Playlists.ProjectToList();

        public IList<TidalTrack> Tracks => _vaultContext.Tracks.ProjectToList();

        public TidalArtist GetArtist(int id) => _vaultContext.Artists.FirstOrDefault(a => a.Id == id);

        public TidalAlbum GetAlbum(int id) => _vaultContext.Albums.FirstOrDefault(a => a.Id == id);

        public TidalPlaylist GetPlaylist(string id) => _vaultContext.Playlists.FirstOrDefault(a => a.Uuid == id);

        public TidalTrack GetTrack(int id) => _vaultContext.Tracks.FirstOrDefault(a => a.Id == id);
    }

    public static class Extensions
    {
        public static IList<T> ProjectToList<T>(this IQueryable<T> x) where T : class => x.Select(i => i).ToList();
    }
}