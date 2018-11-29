using System.Linq;
using Clockwork.Vault.Dao;
using Clockwork.Vault.Query.Tidal;

namespace Clockwork.Vault.DataTransfer.TidalToMaster
{
    public class TidalToMasterDataOrchestrator
    {
        public static void TransferArtists(VaultContext context)
        {
            var tidalOrchestrator = new TidalOrchestrator();
            var tidalArtists = tidalOrchestrator.Artists;

            foreach (var tidalArtist in tidalArtists)
            {
                var artist = TidalToMasterDataMapper.Map(tidalArtist);
                MasterDataInserter.InsertArtist(context, artist);
            }

            context.SaveChanges();
        }
    }
}
