using Clockwork.Vault.Dao;
using log4net;
using log4net.Config;
using NUnit.Framework;

namespace Clockwork.Vault.DataTransfer.TidalToMaster.Tests.SystemTests
{
    [TestFixture]
    [Ignore("Integration tests")]
    public class TransferDataTests
    {
        private VaultContext _vaultContext;

        private static readonly ILog Log = LogManager.GetLogger("Default");

        public TransferDataTests()
        {
            XmlConfigurator.Configure();
        }

        [Test]
        public void TransferArtists()
        {
            Log.Info("Starting TransferArtists");

            GetInMemContextOrEstablish();
            TidalToMasterDataOrchestrator.TransferArtists(_vaultContext);
        }

        private void GetInMemContextOrEstablish() => _vaultContext = new VaultContext();
    }
}
