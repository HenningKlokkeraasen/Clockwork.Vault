﻿namespace Clockwork.Vault.Integrations.Tidal.Dao.Models
{
    public class TidalTrack : TidalMusicalWorkBase
    {
        public int TrackNumber { get; set; }
        public int VolumeNumber { get; set; }
        public string Isrc { get; set; }
    }
}