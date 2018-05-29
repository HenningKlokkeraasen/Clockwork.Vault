using System;
using System.Linq;
using Clockwork.Vault.Integrations.Tidal.Dao;
using Clockwork.Vault.Integrations.Tidal.Dao.Models;

namespace Clockwork.Vault.Integrations.Tidal.Orchestration
{
    public static class DbInserter
    {
        public static void InsertCreator(VaultContext context, TidalCreator creator)
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

        public static void InsertPlaylist(VaultContext context, TidalPlaylist playlist)
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

        public static void InsertTrack(VaultContext context, TidalTrack track)
        {
            var existingRecord = context.Tracks.FirstOrDefault(p => p.Id == track.Id);
            if (existingRecord != null)
            {
                Console.WriteLine($"Record exists: track {existingRecord.Id} {track.Title}");
            }
            else
            {
                context.Tracks.Add(track);
                Console.WriteLine($"Inserted track {track.Id} {track.Title}");
            }
        }

        public static void InsertPlaylistTrack(VaultContext context, TidalPlaylistTrack playlistTrack)
        {
            var existingRecord = context.PlaylistTracks
                .FirstOrDefault(p => p.TrackId == playlistTrack.TrackId
                                     && p.PlaylistId == playlistTrack.PlaylistId);
            if (existingRecord != null)
            {
                Console.WriteLine($"Record exists: playlist-track {existingRecord.PlaylistId} {existingRecord.TrackId}");
            }
            else
            {
                context.PlaylistTracks.Add(playlistTrack);
                Console.WriteLine($"Inserted playlist-track {playlistTrack.PlaylistId} {playlistTrack.TrackId}");
            }
        }

        public static void InsertAlbum(VaultContext context, TidalAlbum album)
        {
            var existingRecord = context.Albums.FirstOrDefault(p => p.Id == album.Id);
            if (existingRecord != null)
            {
                Console.WriteLine($"Record exists: album {existingRecord.Id} {album.Title}");
            }
            else
            {
                context.Albums.Add(album);
                Console.WriteLine($"Inserted album {album.Id} {album.Title}");
            }
        }
    }
}