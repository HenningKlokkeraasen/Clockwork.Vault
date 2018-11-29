using System;
using Clockwork.Vault.Dao;
using NUnit.Framework;

namespace Clockwork.Vault.Integrations.Tidal.Tests.DaoTests
{
    [TestFixture]
    [Ignore("needs db context")]
    public class DaoTests
    {
        private VaultContext _vaultContext;

        [Test]
        public void Can_connect()
        {
            GetInMemContextOrEstablish();
        }

        [Test]
        public void Can_get_playlists()
        {
            GetInMemContextOrEstablish();
            foreach (var playlist in _vaultContext.Playlists)
                Console.WriteLine(playlist.Title);
        }

        [Test]
        public void Can_get_tracks()
        {
            GetInMemContextOrEstablish();
            foreach (var track in _vaultContext.Tracks)
                Console.WriteLine(track.Title);
        }

        [Test]
        public void Can_get_albums()
        {
            GetInMemContextOrEstablish();
            foreach (var album in _vaultContext.Albums)
                Console.WriteLine(album.Title);
        }

        [Test]
        public void Can_get_artists()
        {
            GetInMemContextOrEstablish();
            foreach (var artist in _vaultContext.Artists)
                Console.WriteLine(artist.Name);
        }

        private void GetInMemContextOrEstablish() => _vaultContext = new VaultContext();
    }
}