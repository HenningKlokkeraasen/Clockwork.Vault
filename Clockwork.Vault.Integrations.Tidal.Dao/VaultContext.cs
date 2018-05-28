using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace Clockwork.Vault.Integrations.Tidal.Dao
{
    public class VaultContext : DbContext
    {
        public VaultContext() : base("MyDB")
        {
        }

        public DbSet<TidalTrack> Tracks { get; set; }
        public DbSet<TidalAlbum> Albums { get; set; }
        public DbSet<TidalArtist> Artists { get; set; }
        public DbSet<TidalPlaylist> Playlists { get; set; }
    }

    public abstract class TidalBase
    {
        [Key]
        public int Id { get; set; }
    }

    public class TidalTrack : TidalBase
    {
        public string Title { get; set; }
    }

    public class TidalAlbum : TidalBase
    {

        public string Title { get; set; }
    }

    public class TidalPlaylist : TidalBase
    {

        public string Title { get; set; }
    }

    public class TidalArtist : TidalBase
    {
        public string Name { get; set; }
    }
}
