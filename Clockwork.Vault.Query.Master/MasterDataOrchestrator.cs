using System.Collections.Generic;
using Clockwork.Vault.Dao.Models.Master;

namespace Clockwork.Vault.Query.Master
{
    public class MasterDataOrchestrator
    {
        private readonly MasterDataRepository _masterDataRepository;

        public MasterDataOrchestrator()
        {
            _masterDataRepository = new MasterDataRepository();
        }

        public IList<Artist> Artists => _masterDataRepository.Artists;
    }
}