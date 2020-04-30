using System.Configuration;
using System.Threading.Tasks;
using Clockwork.Vault.Dao;
using Clockwork.Vault.Integrations.Tidal.Orchestration;
using log4net;
using log4net.Config;
using NUnit.Framework;
using OpenTidl.Methods;

namespace Clockwork.Vault.Integrations.Tidal.Tests.SystemTests
{
    [TestFixture]
    [Ignore("Integration tests")]
    public class SaveDataTests
    {
        private OpenTidlSession _openTidlSession;
        private VaultContext _vaultContext;
        private SaveTidalDataOrchestrator _orchestrator;
        private TidalIntegrator _tidalIntegrator;

        private static readonly ILog Log = LogManager.GetLogger("Default");

        public SaveDataTests()
        {
            XmlConfigurator.Configure();
            var token = ConfigurationManager.AppSettings["tidal_token"];
            _tidalIntegrator = new TidalIntegrator(token);
            _orchestrator = new SaveTidalDataOrchestrator(token);
        }

        [Test]
        public async Task SavePlaylists()
        {
            Log.Info("Starting SavePlaylists");

            GetInMemContextOrEstablish();
            await GetInMemSessionOrLoginAsync();
            await _orchestrator.SavePlaylists(_openTidlSession, _vaultContext);
        }

        [Test]
        public async Task SaveUserFavPlaylists()
        {
            Log.Info("Starting SaveUserFavPlaylists");

            GetInMemContextOrEstablish();
            await GetInMemSessionOrLoginAsync();
            await _orchestrator.SaveUserFavPlaylists(_openTidlSession, _vaultContext);
        }

        [Test]
        public async Task SaveUserFavAlbums()
        {
            Log.Info("Starting SaveUserFavAlbums");

            GetInMemContextOrEstablish();
            await GetInMemSessionOrLoginAsync();
            await _orchestrator.SaveUserFavAlbums(_openTidlSession, _vaultContext);
        }

        [Test]
        public async Task SaveUserFavTracks()
        {
            Log.Info("Starting SaveUserFavTracks");

            GetInMemContextOrEstablish();
            await GetInMemSessionOrLoginAsync();
            await _orchestrator.SaveUserFavTracks(_openTidlSession, _vaultContext);
        }

        [Test]
        public async Task SaveUserFavArtists()
        {
            Log.Info("Starting SaveUserFavArtists");

            GetInMemContextOrEstablish();
            await GetInMemSessionOrLoginAsync();
            await _orchestrator.SaveUserFavArtists(_openTidlSession, _vaultContext);
        }

        [Test]
        public async Task EnsureAlbumUpc()
        {
            Log.Info("Starting Ensure Album UPC");
            GetInMemContextOrEstablish();
            await GetInMemSessionOrLoginAsync();
            var iterationSettings = new IterationSettings
            {
                SleepTimeInSeconds = 1
            };
            await _orchestrator.EnsureAlbumUpc(_vaultContext, iterationSettings);
        }

        [Test]
        public async Task EnsureTrackIsrc()
        {
            Log.Info("Starting Ensure Track ISRC");
            GetInMemContextOrEstablish();
            await GetInMemSessionOrLoginAsync();
            var iterationSettings = new IterationSettings
            {
                SleepTimeInSeconds = 1
            };
            await _orchestrator.EnsureTrackIsrc(_vaultContext, iterationSettings);
        }

        private void GetInMemContextOrEstablish() => _vaultContext = new VaultContext();

        private async Task GetInMemSessionOrLoginAsync() 
            => _openTidlSession = _openTidlSession ?? await LoginUserAsync();

        private Task<OpenTidlSession> LoginUserAsync()
        {
            var appSettingsReader = new AppSettingsReader();
            var username = appSettingsReader.GetValue("tidal.username", typeof(string)) as string;
            var password = appSettingsReader.GetValue("tidal.password", typeof(string)) as string;
            return _tidalIntegrator.LoginUserAsync(username, password);
        }
    }
}