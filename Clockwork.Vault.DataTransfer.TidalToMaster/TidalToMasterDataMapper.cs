using Clockwork.Vault.Dao.Models.Master;
using Clockwork.Vault.Dao.Models.Tidal;

namespace Clockwork.Vault.DataTransfer.TidalToMaster
{
    public class TidalToMasterDataMapper
    {
        public static Artist Map(TidalArtist tidalArtist)
        {
            return new Artist
            {
                Source = SourceEnum.Tidal,
                SourceId = tidalArtist.Id,

                Name = tidalArtist.Name
            };
        }

        public static Album Map(TidalAlbum tidalAlbum)
        {
            return new Album
            {
                Source = SourceEnum.Tidal,
                SourceId = tidalAlbum.Id,
                
                Title = tidalAlbum.Title,
                Version = tidalAlbum.Version,
                Duration = tidalAlbum.Duration,
                NumberOfTracks = tidalAlbum.NumberOfTracks,
                NumberOfVolumes = tidalAlbum.NumberOfVolumes,
                ReleaseDate = tidalAlbum.ReleaseDate,
                Type = tidalAlbum.Type,
                Upc = tidalAlbum.Upc,
                Cover = tidalAlbum.Cover
            };
        }

        public static Track Map(TidalTrack tidalTrack)
        {
            return new Track
            {
                Source = SourceEnum.Tidal,

                Title = tidalTrack.Title,
                Version = tidalTrack.Version,
                Duration = tidalTrack.Duration,
                TrackNumber = tidalTrack.TrackNumber,
                VolumeNumber = tidalTrack.VolumeNumber,
                Isrc = tidalTrack.Isrc
            };
        }

        public static Playlist Map(TidalPlaylist tidalPlaylist)
        {
            return new Playlist
            {
                Source = SourceEnum.Tidal,
                SourceId = tidalPlaylist.Uuid,

                Type = tidalPlaylist.Type,
                IsPublic = tidalPlaylist.PublicPlaylist,
                Title = tidalPlaylist.Title,
                Description = tidalPlaylist.Description,
                Created = tidalPlaylist.Created,
                LastUpdated = tidalPlaylist.LastUpdated,
                NumberOfTracks = tidalPlaylist.NumberOfTracks,
                Duration = tidalPlaylist.Duration,
                CreatorId = tidalPlaylist.CreatorId
            };
        }
    }
}