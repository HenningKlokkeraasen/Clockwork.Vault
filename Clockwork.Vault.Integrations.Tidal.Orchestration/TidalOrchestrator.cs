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
    }

    public static class Extensions
    {
        public static IList<T> ProjectToList<T>(this IQueryable<T> x) where T : class => x.Select(i => i).ToList();
    }
}