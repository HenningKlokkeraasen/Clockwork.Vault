using System;
using System.Threading.Tasks;
using OpenTidl;
using OpenTidl.Methods;
using OpenTidl.Models;

namespace Clockwork.Vault.Integrations.Tidal
{
    public static class TidalIntegrator
    {
        private static readonly OpenTidlClient Client = new OpenTidlClient(ClientConfiguration.Default);

        public static async Task<OpenTidlSession> LoginUserAsync(string username, string password)
        {
            OpenTidlSession session;
            try
            {
                session = await Client.LoginWithUsername(username, password);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
            return session;
        }

        public static async Task<AlbumModel> GetAlbum(int id)
        {
            AlbumModel album;
            try
            {
                album = await Client.GetAlbum(id);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
            return album;
        }

        public static async Task<ArtistModel> GetArtist(int id)
        {
            ArtistModel artist;
            try
            {
                artist = await Client.GetArtist(id);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
            return artist;
        }
    }
}
