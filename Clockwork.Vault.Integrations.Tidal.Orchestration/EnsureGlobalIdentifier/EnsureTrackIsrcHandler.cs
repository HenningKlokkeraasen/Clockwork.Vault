using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Clockwork.Vault.Dao;

namespace Clockwork.Vault.Integrations.Tidal.Orchestration.EnsureGlobalIdentifier
{
    public class EnsureTrackIsrcHandler
    {
        private readonly TidalIntegrator _tidalIntegrator;
        private readonly VaultContext _vaultContext;

        public EnsureTrackIsrcHandler(TidalIntegrator tidalIntegrator, VaultContext vaultContext)
        {
            _tidalIntegrator = tidalIntegrator;
            _vaultContext = vaultContext;
        }

        public async Task<IList<string>> Run(IterationSettings iterationSettings)
        {
            var log = new List<string>();

            var queryable = from a in _vaultContext.TidalTracks.Where(a => a.Isrc == null)
                            select a;
            // https://stackoverflow.com/questions/2113498/sqlexception-from-entity-framework-new-transaction-is-not-allowed-because-ther
            var tracksWithoutIsrc = queryable.ToList();

            var sleepTimeInSeconds = iterationSettings?.SleepTimeInSeconds > 0
                ? iterationSettings.SleepTimeInSeconds
                : 0;

            foreach (var tidalTrack in tracksWithoutIsrc)
            {
                var trackResult = await _tidalIntegrator.GetTrack(tidalTrack.Id);

                if (trackResult == null)
                {
                    log.Add($"WARN Could not get track {tidalTrack.Id} {tidalTrack.Title}");
                }
                else
                {
                    var album = TidalDaoMapper.MapTidalTrackModelToDao(trackResult);
                    TidalDbInserter.UpdateFields(_vaultContext, album, tidalTrack);
                }

                log.Add($"Sleeping for {sleepTimeInSeconds} seconds");
                Thread.Sleep(sleepTimeInSeconds * 1000);
            }

            return log;
        }
    }
}