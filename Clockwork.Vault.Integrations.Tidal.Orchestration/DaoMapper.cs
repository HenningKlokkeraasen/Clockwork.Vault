using System;
using Clockwork.Vault.Integrations.Tidal.Dao.Models;
using OpenTidl.Models;
using OpenTidl.Models.Base;

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

        public static TidalPlaylistTrack MapTidalPlaylistTrackDao(TidalTrack track, TidalPlaylist playlist, int position)
        {
            var dbItem = new TidalPlaylistTrack
            {
                PlaylistId = playlist.Uuid,
                TrackId = track.Id,
                Position = position
            };
            return dbItem;
        }

        public static TidalAlbum MapTidalAlbumModelToDao(AlbumModel item)
        {
            var dbItem = new TidalAlbum
            {
                Id = item.Id,
                Title = item.Title,
                Version = item.Version,
                ReleaseDate = item.ReleaseDate ?? new DateTime(1900, 1, 1),
                Type = item.Type,
                Cover = item.Cover,
                NumberOfTracks = item.NumberOfTracks,
                NumberOfVolumes = item.NumberOfVolumes,
                Duration = item.Duration,
                Copyright = item.Copyright,
                StreamReady = item.StreamReady,
                AllowStreaming = item.AllowStreaming,
                StreamStartDate = item.StreamStartDate ?? new DateTime(1900, 1, 1),
                PremiumStreamingOnly = item.PremiumStreamingOnly,
                Upc = null, // TODO Need updated OpenTidl
                AudioQuality = null // TODO Need updated OpenTidl
            };
            return dbItem;
        }

        public static TidalArtist MapTidalArtistModelToDao(ArtistModel item)
        {
            var dbItem = new TidalArtist
            {
                Id = item.Id,
                Name = item.Name
            };
            return dbItem;
        }

        public static TidalTrackArtist MapTidalTrackArtistDao(TrackModel track, ArtistModel artist)
        {
            var dbItem = new TidalTrackArtist
            {
                ArtistId = artist.Id,
                TrackId = track.Id,
                Type = artist.Type
            };
            return dbItem;
        }

        public static TidalAlbumArtist MapTidalAlbumArtistDao(AlbumModel album, ArtistModel artist)
        {
            var dbItem = new TidalAlbumArtist
            {
                ArtistId = artist.Id,
                AlbumId = album.Id,
                Type = artist.Type
            };
            return dbItem;
        }

        public static TidalUserFavoritePlaylist MapTidalPlaylistFavDao(JsonListItem<PlaylistModel> jsonListItem)
        {
            var dbItem = new TidalUserFavoritePlaylist
            {
                PlaylistId = jsonListItem.Item.Uuid,
                Created = jsonListItem.Created ?? new DateTime(1900, 1, 1)
            };
            return dbItem;
        }

        public static TidalUserFavoriteAlbum MapTidalAlbumFavDao(JsonListItem<AlbumModel> jsonListItem)
        {
            var dbItem = new TidalUserFavoriteAlbum
            {
                AlbumId = jsonListItem.Item.Id,
                Created = jsonListItem.Created ?? new DateTime(1900, 1, 1)
            };
            return dbItem;
        }

        public static TidalUserFavoriteTrack MapTidalTrackFavDao(JsonListItem<TrackModel> jsonListItem)
        {
            var dbItem = new TidalUserFavoriteTrack
            {
                TrackId = jsonListItem.Item.Id,
                Created = jsonListItem.Created ?? new DateTime(1900, 1, 1)
            };
            return dbItem;
        }

        public static TidalUserFavoriteArtist MapTidalArtistFavDao(JsonListItem<ArtistModel> jsonListItem)
        {
            var dbItem = new TidalUserFavoriteArtist
            {
                ArtistId = jsonListItem.Item.Id,
                Created = jsonListItem.Created ?? new DateTime(1900, 1, 1)
            };
            return dbItem;
        }
    }
}