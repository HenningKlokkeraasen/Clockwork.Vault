﻿using System.Data.Entity;
using Clockwork.Vault.Integrations.Tidal.Dao.Migrations;
using Clockwork.Vault.Integrations.Tidal.Dao.Models;

namespace Clockwork.Vault.Integrations.Tidal.Dao
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


    }
}
