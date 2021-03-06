﻿using Clockwork.Vault.Dao.Models.Core;

namespace Clockwork.Vault.Dao.Models.Tidal
{
    public abstract class TidalArtistRelationBase : AppGeneratedEntityBase
    {
        public int ArtistId { get; set; }
        public virtual TidalArtist Artist { get; set; }

        public string Type { get; set; }
    }
}