using System.Linq;
using Clockwork.Vault.Dao;
using Clockwork.Vault.Dao.Models.Master;

namespace Clockwork.Vault.DataTransfer.TidalToMaster
{
    public class MasterDataInserter
    {
        private readonly VaultContext _context;

        public MasterDataInserter(VaultContext context)
        {
            _context = context;
        }

        public string InsertArtist(Artist artist)
        {
            var existingRecord = _context.Artists.FirstOrDefault(p => p.SourceId == artist.SourceId && p.Source == artist.Source);

            if (existingRecord != null)
                return $"Record exists: artist with name {existingRecord.Name} from source {artist.Source}, ID {artist.SourceId}";

            _context.Artists.Add(artist);

            return $"Inserted artist {artist.Name}";
        }

        public string InsertAlbum(Album album)
        {
            var exactMatch = _context.Albums.FirstOrDefault(p => p.Upc == album.Upc);

            if (exactMatch != null)
                return $"Record exists: album with title {exactMatch.Title}";

            var existingRecord = _context.Albums.FirstOrDefault(p => p.SourceId == album.SourceId && p.Source == album.Source);

            if (existingRecord != null)
                return $"Record exists: album with title {existingRecord.Title}";

            _context.Albums.Add(album);

            return $"Inserted album {album.Title}";
        }

        public string InsertTrack(Track track)//, IEnumerable<TrackArtist> trackArtists
        {
            var exactMatch = _context.Tracks.FirstOrDefault(p => p.Isrc == track.Isrc);

            if (exactMatch != null)
                return $"Record exists: track with title {exactMatch.Title}";

            var existingRecord = _context.Tracks.FirstOrDefault(p => p.SourceId == track.SourceId && p.Source == track.Source);

            if (existingRecord != null)
                return $"Record exists: track with title {existingRecord.Title}";

            track.PossiblyDuplicate = true;

            _context.Tracks.Add(track);

            return $"Inserted track {track.Title} [POSSIBLY DUPLICATE]";
        }

        public string InsertPlaylist(Playlist playlist)
        {
            var exactMatch = _context.Playlists.FirstOrDefault(p => p.SourceId == playlist.SourceId
                                                                    && p.Source == playlist.Source);

            if (exactMatch != null)
                return $"Record exists: track with title {exactMatch.Title}";

            _context.Playlists.Add(playlist);

            return $"Inserted playlist {playlist.Title}";
        }
    }
}