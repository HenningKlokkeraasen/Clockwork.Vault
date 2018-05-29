using System.Configuration;
using System.Threading.Tasks;
using Clockwork.Vault.Integrations.Tidal.Dao;
using Clockwork.Vault.Integrations.Tidal.Orchestration;
using NUnit.Framework;
using OpenTidl.Methods;

namespace Clockwork.Vault.Integrations.Tidal.Tests.SystemTests
{
    [TestFixture]
    public class SaveDataTests
    {
        private OpenTidlSession _openTidlSession;
        private VaultContext _vaultContext;

        [Test]
        public async Task SavePlaylists()
        {
            GetInMemContextOrEstablish();
            await GetInMemSessionOrLoginAsync();
            await SaveTidalDataOrchestrator.SavePlaylists(_openTidlSession, _vaultContext);
        }

        private void GetInMemContextOrEstablish() => _vaultContext = new VaultContext();

        private async Task GetInMemSessionOrLoginAsync() 
            => _openTidlSession = _openTidlSession ?? await LoginUserAsync();

        private static Task<OpenTidlSession> LoginUserAsync()
        {
            var appSettingsReader = new AppSettingsReader();
            var username = appSettingsReader.GetValue("tidal.username", typeof(string)) as string;
            var password = appSettingsReader.GetValue("tidal.password", typeof(string)) as string;
            return TidalIntegrator.LoginUserAsync(username, password);
        }
    }
}