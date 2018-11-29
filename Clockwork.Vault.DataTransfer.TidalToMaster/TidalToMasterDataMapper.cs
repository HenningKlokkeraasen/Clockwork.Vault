using Clockwork.Vault.Dao.Models.Master;
using Clockwork.Vault.Dao.Models.Tidal;

namespace Clockwork.Vault.DataTransfer.TidalToMaster
{
    public class TidalToMasterDataMapper
    {
        public static Artist Map(TidalArtist tidalArtist)
        {
            return new Artist
            {
                Name = tidalArtist.Name,
                Source = SourceEnum.Tidal
            };
        }
    }
}