using System;
using System.Threading.Tasks;
using log4net;
using OpenTidl;
using OpenTidl.Methods;
using OpenTidl.Models;
using OpenTidl.Models.Base;

namespace Clockwork.Vault.Integrations.Tidal
{
    public static class TidalIntegrator
    {
        private static readonly OpenTidlClient Client = new OpenTidlClient(ClientConfiguration.Default);

        private static readonly ILog Log = LogManager.GetLogger("Default");

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
                Log.Error(e);
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
                var errorMessage = e.InnerException?.Message ?? e.Message;
                Console.WriteLine(errorMessage);
                Log.Error(errorMessage);
                return null;
            }
            return album;
        }

        public static async Task<JsonList<TrackModel>> GetAlbumTracks(int albumId)
        {
            JsonList<TrackModel> tracks;
            try
            {
                tracks = await Client.GetAlbumTracks(albumId);
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

        public static async Task<TrackModel> GetTrack(int id)
        {
            TrackModel track;
            try
            {
                track = await Client.GetTrack(id);
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
    }
}
