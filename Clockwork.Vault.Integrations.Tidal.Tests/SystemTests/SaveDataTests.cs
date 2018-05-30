﻿using System.Configuration;
using System.Threading.Tasks;
using Clockwork.Vault.Integrations.Tidal.Dao;
using Clockwork.Vault.Integrations.Tidal.Orchestration;
using log4net;
using log4net.Config;
using NUnit.Framework;
using OpenTidl.Methods;

namespace Clockwork.Vault.Integrations.Tidal.Tests.SystemTests
{
    [TestFixture]
    public class SaveDataTests
    {
        private OpenTidlSession _openTidlSession;
        private VaultContext _vaultContext;

        private static readonly ILog Log = LogManager.GetLogger("Default");

        public SaveDataTests()
        {
            XmlConfigurator.Configure();
        }

        [Test]
        public async Task SavePlaylists()
        {
            Log.Info("Starting SavePlaylists");

            GetInMemContextOrEstablish();
            await GetInMemSessionOrLoginAsync();
            await SaveTidalDataOrchestrator.SavePlaylists(_openTidlSession, _vaultContext);
        }

        [Test]
        public async Task SaveUserFavPlaylists()
        {
            Log.Info("Starting SaveUserFavPlaylists");

            GetInMemContextOrEstablish();
            await GetInMemSessionOrLoginAsync();
            await SaveTidalDataOrchestrator.SaveUserFavPlaylists(_openTidlSession, _vaultContext);
        }

        [Test]
        public async Task SaveUserFavTracks()
        {
            Log.Info("Starting SaveUserFavTracks");

            GetInMemContextOrEstablish();
            await GetInMemSessionOrLoginAsync();
            await SaveTidalDataOrchestrator.SaveUserFavTracks(_openTidlSession, _vaultContext);
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