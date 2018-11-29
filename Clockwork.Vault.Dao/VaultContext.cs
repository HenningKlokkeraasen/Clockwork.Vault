using System.Data.Entity;
using Clockwork.Vault.Dao.Migrations;
using Clockwork.Vault.Dao.Models.Tidal;

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

        public DbSet<TidalTrack> Tracks { get; set; }
        public DbSet<TidalAlbum> Albums { get; set; }
        public DbSet<TidalArtist> Artists { get; set; }
        public DbSet<TidalPlaylist> Playlists { get; set; }
        public DbSet<TidalCreator> Creators { get; set; }

        public DbSet<TidalAlbumArtist> AlbumArtists { get; set; }
        public DbSet<TidalAlbumTrack> AlbumTracks { get; set; }
        public DbSet<TidalTrackArtist> TrackArtists { get; set; }
        public DbSet<TidalPlaylistTrack> PlaylistTracks { get; set; }

        public DbSet<TidalUserFavoriteAlbum> FavoriteAlbums { get; set; }
        public DbSet<TidalUserFavoriteTrack> FavoriteTracks { get; set; }
        public DbSet<TidalUserFavoriteArtist> FavoriteArtists { get; set; }
        public DbSet<TidalUserFavoritePlaylist> FavoritePlaylists { get; set; }
    }
}
