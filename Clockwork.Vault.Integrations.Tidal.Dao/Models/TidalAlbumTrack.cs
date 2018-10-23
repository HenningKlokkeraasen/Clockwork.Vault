﻿namespace Clockwork.Vault.Integrations.Tidal.Dao.Models
{
    public class TidalAlbumTrack : TidalTrackRelationBase
    {
        public int AlbumId { get; set; }
        public virtual TidalAlbum Album { get; set; }
    }
}