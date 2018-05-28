using System.Configuration;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace Clockwork.Vault.Integrations.Tidal.Tests.IntegrationTests
{
    [TestFixture]
    public class TidalIntegrationTests
    {
        [Test]
        public async Task Can_login_user()
        {
            var appSettingsReader = new AppSettingsReader();
            var username = appSettingsReader.GetValue("tidal.username", typeof(string)) as string;
            var password = appSettingsReader.GetValue("tidal.password", typeof(string)) as string;
            var session = await TidalIntegrator.LoginUserAsync(username, password);
            session.LoginResult.SessionId.Should().NotBeEmpty();
        }
    }
}
