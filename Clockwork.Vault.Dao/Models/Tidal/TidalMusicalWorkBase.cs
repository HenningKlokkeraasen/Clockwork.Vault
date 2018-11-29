using System;

namespace Clockwork.Vault.Dao.Models.Tidal
{
    public abstract class TidalMusicalWorkBase : TidalIntIdBase
    {
        public string Title { get; set; }
        public string Version { get; set; }
        public int Duration { get; set; }
        public string Copyright { get; set; }
        public bool StreamReady { get; set; }
        public bool AllowStreaming { get; set; }
        public bool PremiumStreamingOnly { get; set; }
        public DateTime StreamStartDate { get; set; }
        public string AudioQuality { get; set; }
    }
}