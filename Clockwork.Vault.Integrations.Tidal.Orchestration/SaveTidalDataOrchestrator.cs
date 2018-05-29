using System.Linq;
using System.Threading.Tasks;
using Clockwork.Vault.Integrations.Tidal.Dao;
using OpenTidl.Methods;

namespace Clockwork.Vault.Integrations.Tidal.Orchestration
{
    public static class SaveTidalDataOrchestrator
    {
        public static async Task SavePlaylists(OpenTidlSession session, VaultContext context)
        {
            var result = await session.GetUserPlaylists(5);
            var playlists = result.Items.Select(DaoMapper.MapTidalPlaylistModelToDao);
            var creators = result.Items.Select(DaoMapper.MapTidalCreatorModelToDao);
            var distinctCreators = creators
                .GroupBy(c => c.Id)
                .Select(group => group.First());

            foreach (var creator in distinctCreators)
                DbInserter.InsertCreator(context, creator);

            context.SaveChanges();

            foreach (var playlist in playlists)
                DbInserter.InsertPlaylist(context, playlist);

            context.SaveChanges();

            foreach (var playlist in playlists)
            {
                var tracksResult = await session.GetPlaylistTracks(playlist.Uuid);
                var tracks = tracksResult.Items.Select(DaoMapper.MapTidalTrackModelToDao);
                foreach (var track in tracks)
                {
                    DbInserter.InsertTrack(context, track);
                }
                var playlistTracks = tracks.Select(i => DaoMapper.MapTidalPlaylistTrackDao(i, playlist));
                foreach (var playlistTrack in playlistTracks)
                {
                    DbInserter.InsertPlaylistTrack(context, playlistTrack);
                }
            }

            context.SaveChanges();
        }
    }
}
