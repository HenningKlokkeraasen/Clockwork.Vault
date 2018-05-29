using System;
using System.Configuration;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using OpenTidl.Methods;

namespace Clockwork.Vault.Integrations.Tidal.Tests.IntegrationTests
{
    [TestFixture]
    public class TidalIntegrationTests
    {
        private OpenTidlSession _openTidlSession;
        
        [Test]
        public async Task Can_login_user()
        {
            await GetInMemSessionOrLoginAsync();
            Console.WriteLine("Session ID: " + _openTidlSession.SessionId);
            Console.WriteLine("User ID: " + _openTidlSession.UserId);
            _openTidlSession.LoginResult.SessionId.Should().NotBeEmpty();
        }

        [Test]
        public async Task Can_get_user_playlists()
        {
            await GetInMemSessionOrLoginAsync();
            var jsonList = await _openTidlSession.GetUserPlaylists();
            Console.WriteLine($"Got {jsonList.Items.Length} of {jsonList.TotalNumberOfItems}");
            foreach (var item in jsonList.Items)
            {
                Console.WriteLine(item.Title);
            }
        }

        [Test]
        public async Task Can_get_fav_playlists()
        {
            await GetInMemSessionOrLoginAsync();
            var jsonList = await _openTidlSession.GetFavoritePlaylists();
            Console.WriteLine($"Got {jsonList.Items.Length} of {jsonList.TotalNumberOfItems}");
        }

        [Test]
        public async Task Can_get_fav_tracks()
        {
            await GetInMemSessionOrLoginAsync();
            var jsonList = await _openTidlSession.GetFavoriteTracks();
            Console.WriteLine($"Got {jsonList.Items.Length} of {jsonList.TotalNumberOfItems}");
        }

        [Test]
        public async Task Can_get_fav_albums()
        {
            await GetInMemSessionOrLoginAsync();
            var jsonList = await _openTidlSession.GetFavoriteAlbums();
            Console.WriteLine($"Got {jsonList.Items.Length} of {jsonList.TotalNumberOfItems}");
        }

        [Test]
        public async Task Can_get_fav_artists()
        {
            await GetInMemSessionOrLoginAsync();
            var jsonList = await _openTidlSession.GetFavoriteArtists();
            Console.WriteLine($"Got {jsonList.Items.Length} of {jsonList.TotalNumberOfItems}");
        }

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
