using System;
using Clockwork.Vault.Integrations.Tidal.Dao.Models;
using OpenTidl.Models;

namespace Clockwork.Vault.Integrations.Tidal.Orchestration
{
    public static class DaoMapper
    {
        public static TidalPlaylist MapTidalPlaylistModelToDao(PlaylistModel item)
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

        public static TidalCreator MapTidalCreatorModelToDao(PlaylistModel item)
        {
            var dbItem = new TidalCreator
            {
                Id = item.Creator.Id
            };
            return dbItem;
        }

        public static TidalTrack MapTidalTrackModelToDao(TrackModel item)
        {
            var dbItem = new TidalTrack
            {
                Id = item.Id,
                Title = item.Title,
                Version = item.Version,
                TrackNumber = item.TrackNumber,
                VolumeNumber = item.VolumeNumber,
                Duration = item.Duration,
                Copyright = item.Copyright,
                StreamReady = item.StreamReady,
                AllowStreaming = item.AllowStreaming,
                StreamStartDate = item.StreamStartDate ?? new DateTime(1900, 1, 1),
                PremiumStreamingOnly = item.PremiumStreamingOnly,
                Isrc = null, // TODO Need updated OpenTidl
                AudioQuality = null // TODO Need updated OpenTidl
            };
            return dbItem;
        }

        public static TidalPlaylistTrack MapTidalPlaylistTrackDao(TidalTrack track, TidalPlaylist playlist)
        {
            var dbItem = new TidalPlaylistTrack
            {
                PlaylistId = playlist.Uuid,
                TrackId = track.Id,
            };
            return dbItem;
        }
    }
}