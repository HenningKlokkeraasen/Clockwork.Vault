using System.Collections.Generic;
using Clockwork.Vault.Dao;
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

        public MasterDataOrchestrator(VaultContext context)
        {
            _masterDataRepository = new MasterDataRepository(context);  
        }

        public IList<Artist> Artists => _masterDataRepository.Artists;

        public IList<Artist> GetArtists(SourceEnum source, int sourceId) => _masterDataRepository.GetArtists(source, sourceId);

        public IList<Album> GetAlbums(SourceEnum source, int sourceId) => _masterDataRepository.GetAlbums(source, sourceId);
    }
}