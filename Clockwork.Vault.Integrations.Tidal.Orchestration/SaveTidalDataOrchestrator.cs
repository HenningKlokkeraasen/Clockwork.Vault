using System;
using System.Linq;
using System.Threading.Tasks;
using Clockwork.Vault.Integrations.Tidal.Dao;
using Clockwork.Vault.Integrations.Tidal.Dao.Models;
using OpenTidl.Methods;
using OpenTidl.Models;

namespace Clockwork.Vault.Integrations.Tidal.Orchestration
{
    public static class SaveTidalDataOrchestrator
    {
        public static async Task SavePlaylists(OpenTidlSession session, VaultContext context)
        {
            var result = await session.GetUserPlaylists(5);
            var playlists = result.Items.Select(MapTidalPlaylistModelToDao);
            var creators = result.Items.Select(MapTidalCreatorModelToDao);
            var distinctCreators = creators
                .GroupBy(c => c.Id)
                .Select(group => group.First());

            foreach (var creator in distinctCreators)
            {
                var existingRecord = context.Creators.FirstOrDefault(p => p.Id == creator.Id);
                if (existingRecord != null)
                {
                    Console.WriteLine($"Record exists: creator {existingRecord.Id}");
                }
                else
                {
                    context.Creators.Add(creator);
                    Console.WriteLine($"Inserted creator {creator.Id}");
                }
            }
            context.SaveChanges();

            foreach (var playlist in playlists)
            {
                var existingRecord = context.Playlists.FirstOrDefault(p => p.Uuid == playlist.Uuid);
                if (existingRecord != null)
                {
                    Console.WriteLine($"Record exists: playlist {existingRecord.Uuid} {existingRecord.Title}");
                }
                else
                {
                    context.Playlists.Add(playlist);
                    Console.WriteLine($"Inserted playlist {playlist.Uuid} {playlist.Title}");
                }
            }
            context.SaveChanges();
        }

        private static TidalPlaylist MapTidalPlaylistModelToDao(PlaylistModel item)
        {
            var dbItem = new TidalPlaylist
            {
                Title = item.Title,
                Description = item.Description,
                Created = item.Created,
                LastUpdated = item.LastUpdated,
                Duration = item.Duration,
                NumberOfTracks = item.NumberOfTracks,
                Type = item.Type.ToString(),
                Uuid = item.Uuid,
                PublicPlaylist = item.PublicPlaylist,
                CreatorId = item.Creator.Id
            };
            return dbItem;
        }

        private static TidalCreator MapTidalCreatorModelToDao(PlaylistModel item)
        {
            var dbItem = new TidalCreator
            {
                Id = item.Creator.Id
            };
            return dbItem;
        }
    }
}
