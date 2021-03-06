﻿using Clockwork.Vault.Dao.Models.Core;

namespace Clockwork.Vault.Dao.Models.Tidal
{
    public abstract class TidalTrackRelationBase : AppGeneratedEntityBase
    {
        public int TrackId { get; set; }
        public virtual TidalTrack Track { get; set; }

        public int Position { get; set; }
    }
}