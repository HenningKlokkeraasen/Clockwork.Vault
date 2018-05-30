﻿using System.Linq;
using Clockwork.Vault.Integrations.Tidal.Dao;
using Clockwork.Vault.Integrations.Tidal.Dao.Models;
using log4net;

namespace Clockwork.Vault.Integrations.Tidal.Orchestration
{
    public static class DbInserter
    {
        private static readonly ILog Log = LogManager.GetLogger("Default");

        public static void InsertCreator(VaultContext context, TidalCreator creator)
        {
            var existingRecord = context.Creators.FirstOrDefault(p => p.Id == creator.Id);
            if (existingRecord != null)
            {
                Log.Info($"Record exists: creator {existingRecord.Id}");
            }
            else
            {
                context.Creators.Add(creator);
                Log.Info($"Inserted creator {creator.Id}");
            }
        }

        public static void InsertPlaylist(VaultContext context, TidalPlaylist playlist)
        {
            var existingRecord = context.Playlists.FirstOrDefault(p => p.Uuid == playlist.Uuid);
            if (existingRecord != null)
            {
                Log.Info($"Record exists: playlist {existingRecord.Uuid} {existingRecord.Title}");
            }
            else
            {
                context.Playlists.Add(playlist);
                Log.Info($"Inserted playlist {playlist.Uuid} {playlist.Title}");
            }
        }

        public static void InsertTrack(VaultContext context, TidalTrack track)
        {
            var existingRecord = context.Tracks.FirstOrDefault(p => p.Id == track.Id);
            if (existingRecord != null)
            {
                Log.Info($"Record exists: track {existingRecord.Id} {track.Title}");
            }
            else
            {
                context.Tracks.Add(track);
                Log.Info($"Inserted track {track.Id} {track.Title}");
            }
        }

        public static void InsertPlaylistTrack(VaultContext context, TidalPlaylistTrack playlistTrack)
        {
            var existingRecord = context.PlaylistTracks
                .FirstOrDefault(p => p.TrackId == playlistTrack.TrackId
                                     && p.PlaylistId == playlistTrack.PlaylistId);
            if (existingRecord != null)
            {
                Log.Info($"Record exists: playlist-track {existingRecord.PlaylistId} {existingRecord.TrackId}");
            }
            else
            {
                context.PlaylistTracks.Add(playlistTrack);
                Log.Info($"Inserted playlist-track {playlistTrack.PlaylistId} {playlistTrack.TrackId}");
            }
        }

        public static void InsertAlbum(VaultContext context, TidalAlbum album)
        {
            var existingRecord = context.Albums.FirstOrDefault(p => p.Id == album.Id);
            if (existingRecord != null)
            {
                Log.Info($"Record exists: album {existingRecord.Id} {album.Title}");
            }
            else
            {
                context.Albums.Add(album);
                Log.Info($"Inserted album {album.Id} {album.Title}");
            }
        }

        public static void InsertArtist(VaultContext context, TidalArtist artist)
        {
            var existingRecord = context.Artists.FirstOrDefault(p => p.Id == artist.Id);
            if (existingRecord != null)
            {
                Log.Info($"Record exists: artist {existingRecord.Id} {artist.Name}");
            }
            else
            {
                context.Artists.Add(artist);
                Log.Info($"Inserted artist {artist.Id} {artist.Name}");
            }
        }

        public static void InsertTrackArtist(VaultContext context, TidalTrackArtist trackArtist)
        {
            var existingRecord = context.TrackArtists
                .FirstOrDefault(p => p.TrackId == trackArtist.TrackId
                                     && p.ArtistId == trackArtist.ArtistId);
            if (existingRecord != null)
            {
                Log.Info($"Record exists: track-artist {existingRecord.TrackId} {existingRecord.ArtistId}");
            }
            else
            {
                context.TrackArtists.Add(trackArtist);
                Log.Info($"Inserted track-artist {trackArtist.TrackId} {trackArtist.ArtistId}");
            }
        }

        public static void InsertAlbumArtist(VaultContext context, TidalAlbumArtist albumArtist)
        {
            var existingRecord = context.AlbumArtists
                .FirstOrDefault(p => p.AlbumId == albumArtist.AlbumId
                                     && p.ArtistId == albumArtist.ArtistId);
            if (existingRecord != null)
            {
                Log.Info($"Record exists: album-artist {existingRecord.AlbumId} {existingRecord.ArtistId}");
            }
            else
            {
                context.AlbumArtists.Add(albumArtist);
                Log.Info($"Inserted album-artist {albumArtist.AlbumId} {albumArtist.ArtistId}");
            }
        }

        public static void InsertFavPlaylist(VaultContext context, TidalUserFavoritePlaylist fav)
        {
            var existingRecord = context.FavoritePlaylists.FirstOrDefault(p => p.PlaylistId == fav.PlaylistId);
            if (existingRecord != null)
            {
                Log.Info($"Record exists: playlist-fav {existingRecord.PlaylistId}");
            }
            else
            {
                context.FavoritePlaylists.Add(fav);
                Log.Info($"Inserted playlist-fav {fav.PlaylistId}");
            }
        }
        public static void InsertFavTrack(VaultContext context, TidalUserFavoriteTrack fav)
        {
            var existingRecord = context.FavoriteTracks.FirstOrDefault(p => p.TrackId == fav.TrackId);
            if (existingRecord != null)
            {
                Log.Info($"Record exists: track-fav {existingRecord.TrackId}");
            }
            else
            {
                context.FavoriteTracks.Add(fav);
                Log.Info($"Inserted track-fav {fav.TrackId}");
            }
        }

    }
}