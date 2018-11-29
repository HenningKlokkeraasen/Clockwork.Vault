using System.Data.Entity;
using Clockwork.Vault.Dao.Models.Master;
using Clockwork.Vault.Dao.Models.Tidal;
using Clockwork.Vault.Integrations.Tidal.Dao.Migrations;

namespace Clockwork.Vault.Dao
{
    public class VaultContext : DbContext
    {
        public VaultContext() : base("MyDB")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<VaultContext, VaultConfiguration>());

            base.OnModelCreating(modelBuilder);
        }

        // Master

        public DbSet<Track> Tracks { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Playlist> Playlists { get; set; }

        public DbSet<AlbumArtist> AlbumArtists { get; set; }
        public DbSet<AlbumTrack> AlbumTracks { get; set; }
        public DbSet<TrackArtist> TrackArtists { get; set; }
        public DbSet<PlaylistTrack> PlaylistTracks { get; set; }

        public DbSet<FavoriteAlbum> FavoriteAlbums { get; set; }
        public DbSet<FavoriteTrack> FavoriteTracks { get; set; }
        public DbSet<FavoriteArtist> FavoriteArtists { get; set; }
        public DbSet<FavoritePlaylist> FavoritePlaylists { get; set; }

        // Tidal

        public DbSet<TidalTrack> TidalTracks { get; set; }
        public DbSet<TidalAlbum> TidalAlbums { get; set; }
        public DbSet<TidalArtist> TidalArtists { get; set; }
        public DbSet<TidalPlaylist> TidalPlaylists { get; set; }
        public DbSet<TidalCreator> TidalCreators { get; set; }

        public DbSet<TidalAlbumArtist> TidalAlbumArtists { get; set; }
        public DbSet<TidalAlbumTrack> TidalAlbumTracks { get; set; }
        public DbSet<TidalTrackArtist> TidalTrackArtists { get; set; }
        public DbSet<TidalPlaylistTrack> TidalPlaylistTracks { get; set; }

        public DbSet<TidalUserFavoriteAlbum> TidalFavoriteAlbums { get; set; }
        public DbSet<TidalUserFavoriteTrack> TidalFavoriteTracks { get; set; }
        public DbSet<TidalUserFavoriteArtist> TidalFavoriteArtists { get; set; }
        public DbSet<TidalUserFavoritePlaylist> TidalFavoritePlaylists { get; set; }
    }
}
