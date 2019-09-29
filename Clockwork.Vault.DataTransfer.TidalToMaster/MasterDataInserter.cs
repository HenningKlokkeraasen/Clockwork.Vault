using System.Linq;
using Clockwork.Vault.Dao;
using Clockwork.Vault.Dao.Models.Master;
using log4net;

namespace Clockwork.Vault.DataTransfer.TidalToMaster
{
    public class MasterDataInserter
    {
        private readonly VaultContext _context;

        private static readonly ILog Log = LogManager.GetLogger("Default");

        public MasterDataInserter(VaultContext context)
        {
            _context = context;
        }

        public void InsertArtist(Artist artist)
        {
            var existingRecord = _context.Artists.FirstOrDefault(p => p.Name == artist.Name);
            if (existingRecord != null)
            {
                Log.Info($"Record exists: artist with name {existingRecord.Name}");
            }
            else
            {
                _context.Artists.Add(artist);
                Log.Info($"Inserted artist {artist.Name}");
            }
        }

        public void InsertAlbum(Album album)
        {
            var exactMatch = _context.Albums.FirstOrDefault(p => p.Upc == album.Upc);
            if (exactMatch != null)
            {
                Log.Info($"Record exists: album with title {exactMatch.Title}");
            }
            else
            {
                // TODO WIP
                var existingRecord = _context.Albums.FirstOrDefault(p => p.Title == album.Title
                                                                         && p.Version == album.Version
                                                                         && p.Source == album.Source);

                if (existingRecord != null)
                {
                    Log.Info($"Record exists: album with title {existingRecord.Title}");
                }
                else
                {
                    _context.Albums.Add(album);
                    Log.Info($"Inserted album {album.Title}");
                }
            }
        }

        public void InsertTrack(Track track)//, IEnumerable<TrackArtist> trackArtists
        {
            var exactMatch = _context.Tracks.FirstOrDefault(p => p.Isrc == track.Isrc);
            if (exactMatch != null)
            {
                Log.Info($"Record exists: track with title {exactMatch.Title}");
            }
            else
            {
                // TODO WIP
                var existingRecord = _context.Tracks.FirstOrDefault(p => p.Title == track.Title
                                                                         && p.Version == track.Version
                                                                         && p.Source == track.Source);

                if (existingRecord != null)
                {
                    Log.Info($"Record exists: track with title {existingRecord.Title}");
                }
                else
                {
                    track.PossiblyDuplicate = true;
                    _context.Tracks.Add(track);
                    Log.Info($"Inserted track {track.Title} [POSSIBLY DUPLICATE]");
                }
            }
        }

        public void InsertPlaylist(Playlist playlist)
        {
            var exactMatch = _context.Playlists.FirstOrDefault(p => p.SourceId == playlist.SourceId
                                                                    && p.Source == playlist.Source);
            if (exactMatch != null)
            {
                Log.Info($"Record exists: track with title {exactMatch.Title}");
            }
            else
            {
                _context.Playlists.Add(playlist);
                Log.Info($"Inserted playlsit {playlist.Title}");
            }
        }
    }
}