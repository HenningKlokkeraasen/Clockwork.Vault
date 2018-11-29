namespace Clockwork.Vault.Dao.Models.Master
{
    public class FavoriteArtist :FavoriteBase
    {
        public int ArtistId { get; set; }
        public virtual Artist Artist { get; set; }
    }
}