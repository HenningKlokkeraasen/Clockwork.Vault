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

        public async Task<OpenTidlSession> LoginUserAsync(string username, string password)
        {
            OpenTidlSession session;
            try
            {
                session = await _client.LoginWithUsername(username, password);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Log.Error(e);
                return null;
            }
            return session;
        }

        public async Task<AlbumModel> GetAlbum(int id)
        {
            AlbumModel album;
            try
            {
                album = await _client.GetAlbum(id);
            }
            catch (Exception e)
            {
                var errorMessage = e.InnerException?.Message ?? e.Message;
                Console.WriteLine(errorMessage);
                Log.Error(errorMessage);
                return null;
            }
            return album;
        }

        public async Task<JsonList<TrackModel>> GetAlbumTracks(int albumId)
        {
            JsonList<TrackModel> tracks;
            try
            {
                tracks = await _client.GetAlbumTracks(albumId);
            }
            catch (Exception e)
            {
                var errorMessage = e.InnerException?.Message ?? e.Message;
                Console.WriteLine(errorMessage);
                Log.Error(errorMessage);
                return null;
            }

            return tracks;
        }

        public async Task<TrackModel> GetTrack(int id)
        {
            TrackModel track;
            try
            {
                track = await _client.GetTrack(id);
            }
            catch (Exception e)
            {
                var errorMessage = e.InnerException?.Message ?? e.Message;
                Console.WriteLine(errorMessage);
                Log.Error(errorMessage);
                return null;
            }
            return track;
        }

        private static OpenTidlClient MakeClient(string token)
        {
            var defaultConfig = ClientConfiguration.Default;
            var clientConfiguration = new ClientConfiguration(defaultConfig.ApiEndpoint, defaultConfig.UserAgent, token,
                defaultConfig.ClientUniqueKey, defaultConfig.ClientVersion, defaultConfig.DefaultCountryCode);
            return new OpenTidlClient(clientConfiguration);
        }
    }
}
