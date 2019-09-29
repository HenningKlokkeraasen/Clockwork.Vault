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

        private TidalToMasterDataOrchestrator Cut => new TidalToMasterDataOrchestrator(_vaultContext);

        public TransferDataTests()
        {
            XmlConfigurator.Configure();
        }

        [Test]
        public void TransferArtists()
        {
            Log.Info("Starting TransferArtists");

            GetInMemContextOrEstablish();
            Cut.TransferArtists();
        }

        [Test]
        public void TransferAlbums()
        {
            Log.Info("Starting TransferAlbums");
            GetInMemContextOrEstablish();
            Cut.TransferAlbums();
        }

        [Test]
        public void TransferTracks()
        {
            Log.Info("Starting TransferTracks");
            GetInMemContextOrEstablish();
            Cut.TransferTracks();
        }

        [Test]
        public void TransferPlaylists()
        {
            Log.Info("Starting TransferPlaylists");
            GetInMemContextOrEstablish();
            Cut.TransferPlaylists();
        }

        private void GetInMemContextOrEstablish() => _vaultContext = new VaultContext();
    }
}