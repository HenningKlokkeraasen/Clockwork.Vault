using System.Collections.Generic;
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

        internal IList<Artist> Artists => _vaultContext.Artists.ProjectToList();
    }
}