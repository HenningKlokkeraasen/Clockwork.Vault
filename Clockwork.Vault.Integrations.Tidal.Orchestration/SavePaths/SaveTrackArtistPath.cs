using System.Collections.Generic;
using Clockwork.Vault.Integrations.Tidal.Orchestration.SaveCommands;
using OpenTidl.Models;

namespace Clockwork.Vault.Integrations.Tidal.Orchestration.SavePaths
{
    internal class SaveTrackArtistPath : SavePathBase
    {
        public SaveTrackArtistPath(SaveTidalEntityHandler saveTidalEntityHandler)
            : base(saveTidalEntityHandler)
        {
        }

        internal IList<string> Run(TrackModel track, ICollection<ArtistModel> insertedArtists)
        {
            var log = new List<string>();

            // The main artist of the track
            var savedArtist = SaveArtist(insertedArtists, track.Artist);
            if (savedArtist)
                log.Add($"Saved artist {track.Artist.Name}");

            // The artists of the track if multiple
            foreach (var trackArtist in track.Artists)
            {
                var savedArtistN = SaveArtist(insertedArtists, trackArtist);
                if (savedArtistN)
                    log.Add($"Saved artist {trackArtist.Name}");
            }

            // many-to-many table TrackArtists
            _saveTidalEntityHandler.MapAndInsertTrackArtists(track);

            return log;
        }
    }
}