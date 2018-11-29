using Clockwork.Vault.Dao;
using Clockwork.Vault.Query.Tidal;

namespace Clockwork.Vault.DataTransfer.TidalToMaster
{
    public class TidalToMasterDataOrchestrator
    {
        private readonly VaultContext _context;
        private readonly TidalOrchestrator _tidalOrchestrator;
        private readonly MasterDataInserter _masterDataInserter;

        public TidalToMasterDataOrchestrator(VaultContext context)
        {
            _context = context;
            _tidalOrchestrator = new TidalOrchestrator();
            _masterDataInserter = new MasterDataInserter(_context);
        }

        public void TransferArtists()
        {
            var tidalArtists = _tidalOrchestrator.Artists;

            foreach (var tidalArtist in tidalArtists)
            {
                var artist = TidalToMasterDataMapper.Map(tidalArtist);
                _masterDataInserter.InsertArtist(artist);
            }

            _context.SaveChanges();
        }

        public void TransferAlbums()
        {
            var tidalAlbums = _tidalOrchestrator.Albums;

            foreach (var tidalAlbum in tidalAlbums)
            {
                var album = TidalToMasterDataMapper.Map(tidalAlbum);
                _masterDataInserter.InsertAlbum(album);
            }

            _context.SaveChanges();
        }

        public void TransferTracks()
        {
            var tidalTracks = _tidalOrchestrator.Tracks;

            foreach (var tidalTrack in tidalTracks)
            {
                var track = TidalToMasterDataMapper.Map(tidalTrack);
                _masterDataInserter.InsertTrack(track);
            }

            _context.SaveChanges();
        }

        public void TransferPlaylists()
        {
            var tidalPlaylists = _tidalOrchestrator.Playlists;

            foreach (var tidalPlaylist in tidalPlaylists)
            {
                var playlist = TidalToMasterDataMapper.Map(tidalPlaylist);
                _masterDataInserter.InsertPlaylist(playlist);
            }

            _context.SaveChanges();
        }
    }
}
