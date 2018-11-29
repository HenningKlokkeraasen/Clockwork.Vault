using System.Linq;
using Clockwork.Vault.Dao;
using Clockwork.Vault.Dao.Models.Master;
using log4net;

namespace Clockwork.Vault.DataTransfer.TidalToMaster
{
    public class MasterDataInserter
    {
        private static readonly ILog Log = LogManager.GetLogger("Default");

        public static void InsertArtist(VaultContext context, Artist artist)
        {
            var existingRecord = context.Artists.FirstOrDefault(p => p.Name == artist.Name);
            if (existingRecord != null)
            {
                Log.Info($"Record exists: artist with name {existingRecord.Name}");
            }
            else
            {
                context.Artists.Add(artist);
                Log.Info($"Inserted artist {artist.Name}");
            }
        }
    }
}