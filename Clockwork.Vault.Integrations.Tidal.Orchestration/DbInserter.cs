using System.Data.Entity;
using System.Linq;
using Clockwork.Vault.Dao;
using Clockwork.Vault.Dao.Models.Tidal;
using log4net;

namespace Clockwork.Vault.Integrations.Tidal.Orchestration
{
    public static class DbInserter
    {
        private static readonly ILog Log = LogManager.GetLogger("Default");

        public static void InsertCreator(VaultContext context, TidalCreator creator)
        {
            var existingRecord = context.TidalCreators.FirstOrDefault(p => p.Id == creator.Id);
            if (existingRecord != null)
            {
                Log.Info($"Record exists: creator {existingRecord.Id}");
            }
            else
            {
                context.TidalCreators.Add(creator);
                Log.Info($"Inserted creator {creator.Id}");
            }
        }

        public static void InsertPlaylist(VaultContext context, TidalPlaylist playlist)
        {
            var existingRecord = context.TidalPlaylists.FirstOrDefault(p => p.Uuid == playlist.Uuid);
            if (existingRecord != null)
            {
                Log.Info($"Record exists: playlist {existingRecord.Uuid} {existingRecord.Title}");
            }
            else
            {
                context.TidalPlaylists.Add(playlist);
                Log.Info($"Inserted playlist {playlist.Uuid} {playlist.Title}");
            }
        }

        public static void UpsertTrack(VaultContext context, TidalTrack track)
        {
            var existingRecord = context.TidalTracks.FirstOrDefault(p => p.Id == track.Id);
            if (existingRecord != null)
            {
                Log.Info($"Record exists: track {existingRecord.Id} {track.Title}");
                UpdateFields(context, track, existingRecord);
            }
            else
            {
                context.TidalTracks.Add(track);
                Log.Info($"Inserted track {track.Id} {track.Title}");
            }
        }

        /// <summary>
        /// Updates the fields ISRC and AudioQualtiy
        /// </summary>
        public static void UpdateFields(DbContext context, TidalTrack track, TidalTrack existingRecord)
        {
            existingRecord.Isrc = track.Isrc;
            existingRecord.AudioQuality = track.AudioQuality;
            context.SaveChanges();
            Log.Info($"    Updated fields ISRC and AudioQuality for track {existingRecord.Id} {existingRecord.Title}");
        }


        public static void InsertAlbumTrack(VaultContext context, TidalAlbumTrack albumTrack)
        {
            var existingRecord = context.TidalAlbumTracks
                .FirstOrDefault(p => p.TrackId == albumTrack.TrackId
                                     && p.AlbumId == albumTrack.AlbumId);

            if (existingRecord != null)
            {
                Log.Info($"Record exists: album-track {existingRecord.AlbumId} {existingRecord.TrackId}");
            }
            else
            {
                context.TidalAlbumTracks.Add(albumTrack);
                Log.Info($"Inserted album-track {albumTrack.AlbumId} {albumTrack.TrackId}");
            }
        }

        public static void InsertPlaylistTrack(VaultContext context, TidalPlaylistTrack playlistTrack)
        {
            var existingRecord = context.TidalPlaylistTracks
                .FirstOrDefault(p => p.TrackId == playlistTrack.TrackId
                                     && p.PlaylistId == playlistTrack.PlaylistId);

            if (existingRecord != null)
            {
                Log.Info($"Record exists: playlist-track {existingRecord.PlaylistId} {existingRecord.TrackId}");
            }
            else
            {
                context.TidalPlaylistTracks.Add(playlistTrack);
                Log.Info($"Inserted playlist-track {playlistTrack.PlaylistId} {playlistTrack.TrackId}");
            }
        }

        public static void UpsertAlbum(VaultContext context, TidalAlbum album)
        {
            var existingRecord = context.TidalAlbums.FirstOrDefault(p => p.Id == album.Id);
            if (existingRecord != null)
            {
                Log.Info($"Record exists: album {existingRecord.Id} {album.Title}");
                UpdateFields(context, album, existingRecord);
            }
            else
            {
                context.TidalAlbums.Add(album);
                Log.Info($"Inserted album {album.Id} {album.Title}");
            }
        }

        /// <summary>
        /// Updates the fields UPC and AudioQualtiy
        /// </summary>
        public static void UpdateFields(DbContext context, TidalAlbum album, TidalAlbum existingRecord)
        {
            existingRecord.Upc = album.Upc;
            existingRecord.AudioQuality = album.AudioQuality;
            context.SaveChanges();
            Log.Info($"    Updated fields UPC and AudioQuality for album {existingRecord.Id} {existingRecord.Title}");
        }

        public static void InsertArtist(VaultContext context, TidalArtist artist)
        {
            var existingRecord = context.TidalArtists.FirstOrDefault(p => p.Id == artist.Id);
            if (existingRecord != null)
            {
                Log.Info($"Record exists: artist {existingRecord.Id} {artist.Name}");
            }
            else
            {
                context.TidalArtists.Add(artist);
                Log.Info($"Inserted artist {artist.Id} {artist.Name}");
            }
        }

        public static void InsertTrackArtist(VaultContext context, TidalTrackArtist trackArtist)
        {
            var existingRecord = context.TidalTrackArtists
                .FirstOrDefault(p => p.TrackId == trackArtist.TrackId
                                     && p.ArtistId == trackArtist.ArtistId);
            if (existingRecord != null)
            {
                Log.Info($"Record exists: track-artist {existingRecord.TrackId} {existingRecord.ArtistId}");
            }
            else
            {
                context.TidalTrackArtists.Add(trackArtist);
                Log.Info($"Inserted track-artist {trackArtist.TrackId} {trackArtist.ArtistId}");
            }
        }

        public static void InsertAlbumArtist(VaultContext context, TidalAlbumArtist albumArtist)
        {
            var existingRecord = context.TidalAlbumArtists
                .FirstOrDefault(p => p.AlbumId == albumArtist.AlbumId
                                     && p.ArtistId == albumArtist.ArtistId);
            if (existingRecord != null)
            {
                Log.Info($"Record exists: album-artist {existingRecord.AlbumId} {existingRecord.ArtistId}");
            }
            else
            {
                context.TidalAlbumArtists.Add(albumArtist);
                Log.Info($"Inserted album-artist {albumArtist.AlbumId} {albumArtist.ArtistId}");
            }
        }

        public static void InsertFavPlaylist(VaultContext context, TidalUserFavoritePlaylist fav)
        {
            var existingRecord = context.TidalFavoritePlaylists.FirstOrDefault(p => p.PlaylistId == fav.PlaylistId);
            if (existingRecord != null)
            {
                Log.Info($"Record exists: playlist-fav {existingRecord.PlaylistId}");
            }
            else
            {
                context.TidalFavoritePlaylists.Add(fav);
                Log.Info($"Inserted playlist-fav {fav.PlaylistId}");
            }
        }

        public static void InsertFavAlbum(VaultContext context, TidalUserFavoriteAlbum fav)
        {
            var existingRecord = context.TidalFavoriteAlbums.FirstOrDefault(p => p.AlbumId == fav.AlbumId);
            if (existingRecord != null)
            {
                Log.Info($"Record exists: album-fav {existingRecord.AlbumId}");
            }
            else
            {
                context.TidalFavoriteAlbums.Add(fav);
                Log.Info($"Inserted album-fav {fav.AlbumId}");
            }
        }

        public static void InsertFavTrack(VaultContext context, TidalUserFavoriteTrack fav)
        {
            var existingRecord = context.TidalFavoriteTracks.FirstOrDefault(p => p.TrackId == fav.TrackId);
            if (existingRecord != null)
            {
                Log.Info($"Record exists: track-fav {existingRecord.TrackId}");
            }
            else
            {
                context.TidalFavoriteTracks.Add(fav);
                Log.Info($"Inserted track-fav {fav.TrackId}");
            }
        }

        public static void InsertFavArtist(VaultContext context, TidalUserFavoriteArtist fav)
        {
            var existingRecord = context.TidalFavoriteArtists.FirstOrDefault(p => p.ArtistId == fav.ArtistId);
            if (existingRecord != null)
            {
                Log.Info($"Record exists: artist-fav {existingRecord.ArtistId}");
            }
            else
            {
                context.TidalFavoriteArtists.Add(fav);
                Log.Info($"Inserted artist-fav {fav.ArtistId}");
            }
        }
    }
}