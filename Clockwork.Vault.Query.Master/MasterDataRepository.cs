using System.Collections.Generic;
using System.Linq;
using Clockwork.Vault.Dao;
using Clockwork.Vault.Dao.Models.Master;
using Clockwork.Vault.Integrations.Tidal.Orchestration;

namespace Clockwork.Vault.Query.Master
{
    internal class MasterDataRepository
    {
        private readonly VaultContext _vaultContext;

        internal MasterDataRepository()
        {
            _vaultContext = new VaultContext();
        }

        public MasterDataRepository(VaultContext context)
        {
            _vaultContext = context;
        }

        internal IList<Artist> Artists => _vaultContext.Artists.ProjectToList();

        internal IList<Artist> GetArtists(SourceEnum source, int sourceId) =>
            _vaultContext.Artists.Where(a => a.Source == source && a.SourceId == sourceId).ProjectToList();

        internal IList<Album> GetAlbums(SourceEnum source, int sourceId) =>
            _vaultContext.Albums.Where(a => a.Source == source && a.SourceId == sourceId).ProjectToList();
    }
}