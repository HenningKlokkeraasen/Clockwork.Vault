using System;
using System.Threading.Tasks;
using log4net;
using OpenTidl;
using OpenTidl.Methods;
using OpenTidl.Models;
using OpenTidl.Models.Base;

namespace Clockwork.Vault.Integrations.Tidal
{
    public class TidalIntegrator
    {
        private readonly OpenTidlClient _client;
        
        private static readonly ILog Log = LogManager.GetLogger("Default");

        public TidalIntegrator(string token)
        {
            _client = MakeClient(token);
        }

        // API methods using the session (have to be authenticated)

        public async Task<OpenTidlSession> LoginUserAsync(string username, string password)
        {
            OpenTidlSession session = null;
            try
            {
                session = await _client.LoginWithUsername(username, password);
            }
            catch (Exception e)
            {
                LogEx(e);
            }
            return session;
        }

        // API methods using the client (no need for authentication)

        public async Task<AlbumModel> GetAlbum(int id) => await TryGet(_client.GetAlbum, id);

        public async Task<JsonList<TrackModel>> GetAlbumTracks(int albumId) => await TryGet(_client.GetAlbumTracks, albumId);

        public async Task<TrackModel> GetTrack(int id) => await TryGet(_client.GetTrack, id);

        // Private

        private static OpenTidlClient MakeClient(string token)
        {
            var defaultConfig = ClientConfiguration.Default;
            var clientConfiguration = new ClientConfiguration(defaultConfig.ApiEndpoint, defaultConfig.UserAgent, token,
                defaultConfig.ClientUniqueKey, defaultConfig.ClientVersion, defaultConfig.DefaultCountryCode);
            return new OpenTidlClient(clientConfiguration);
        }

        private static async Task<T> TryGet<T>(Func<int, Task<T>> func, int parameter)
        {
            var entity = default(T);
            try
            {
                entity = await func(parameter);
            }
            catch (Exception e)
            {
                LogEx(e);
            }
            return entity;
        }

        private static void LogEx(Exception e) => Log.Error(e.InnerException?.Message ?? e.Message);
    }
}
