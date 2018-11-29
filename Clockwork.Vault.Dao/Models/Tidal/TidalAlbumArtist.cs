namespace Clockwork.Vault.Dao.Models.Tidal
{
    public class TidalAlbumArtist : TidalArtistRelationBase
    {
        public int AlbumId { get; set; }
        public virtual TidalAlbum Album { get; set; }
    }
}