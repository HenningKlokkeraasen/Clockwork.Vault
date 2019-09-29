using System.Collections.Generic;
using System.Linq;
using Clockwork.Vault.Core.Models;
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

        public Log TransferArtists()
        {
            var log = new Log();

            var tidalArtists = _tidalOrchestrator.Artists;

            foreach (var tidalArtist in tidalArtists)
            {
                var artist = TidalToMasterDataMapper.Map(tidalArtist);
                var msg = _masterDataInserter.InsertArtist(artist);
                log.Messages.Add(msg);
            }

            _context.SaveChanges();

            log.Statistics = CalculateStatistics(log.Messages);

            log.Title = "Tidal to Master: Transfer Artists";

            return log;
        }

        public Log TransferAlbums()
        {
            var log = new Log();

            var tidalAlbums = _tidalOrchestrator.Albums;

            foreach (var tidalAlbum in tidalAlbums)
            {
                var album = TidalToMasterDataMapper.Map(tidalAlbum);
                var msg = _masterDataInserter.InsertAlbum(album);
                log.Messages.Add(msg);
            }

            _context.SaveChanges();

            log.Statistics = CalculateStatistics(log.Messages);

            log.Title = "Tidal to Master: Transfer Albums";

            return log;
        }

        public Log TransferTracks()
        {
            var log = new Log();

            var tidalTracks = _tidalOrchestrator.Tracks;

            foreach (var tidalTrack in tidalTracks)
            {
                var track = TidalToMasterDataMapper.Map(tidalTrack);
                var msg = _masterDataInserter.InsertTrack(track);
                log.Messages.Add(msg);
            }

            _context.SaveChanges();

            log.Statistics = CalculateStatistics(log.Messages);

            log.Title = "Tidal to Master: Transfer Tracks";

            return log;
        }

        public Log TransferPlaylists()
        {
            var log = new Log();

            var tidalPlaylists = _tidalOrchestrator.Playlists;

            foreach (var tidalPlaylist in tidalPlaylists)
            {
                var playlist = TidalToMasterDataMapper.Map(tidalPlaylist);
                var msg = _masterDataInserter.InsertPlaylist(playlist);
                log.Messages.Add(msg);
            }

            _context.SaveChanges();

            log.Statistics = CalculateStatistics(log.Messages);

            log.Title = "Tidal to Master: Transfer Playlists";

            return log;
        }

        private static IList<string> CalculateStatistics(IList<string> logMessages)
        {
            var insertedCount = logMessages.Count(msg => msg.StartsWith("Inserted"));
            var existsCount = logMessages.Count(msg => msg.StartsWith("Record exists"));
            var statistics = new List<string>
            {
                $"Inserted: {insertedCount}",
                $"Already existed: {existsCount}"
            };
            return statistics;
        }
    }
}
