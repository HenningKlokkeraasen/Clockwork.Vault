using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Clockwork.Vault.Dao;

namespace Clockwork.Vault.Integrations.Tidal.Orchestration.EnsureGlobalIdentifier
{
    public class EnsureAlbumUpcHandler
    {
        private readonly TidalIntegrator _tidalIntegrator;
        private readonly VaultContext _vaultContext;

        public EnsureAlbumUpcHandler(TidalIntegrator tidalIntegrator, VaultContext vaultContext)
        {
            _tidalIntegrator = tidalIntegrator;
            _vaultContext = vaultContext;
        }

        public async Task<IList<string>> Run(IterationSettings iterationSettings)
        {
            var log = new List<string>();

            var queryable = from a in _vaultContext.TidalAlbums.Where(a => a.Upc == null)
                            select a;
            // https://stackoverflow.com/questions/2113498/sqlexception-from-entity-framework-new-transaction-is-not-allowed-because-ther
            var albumsWithoutUpc = queryable.ToList();

            var sleepTimeInSeconds = iterationSettings?.SleepTimeInSeconds > 0
                ? iterationSettings.SleepTimeInSeconds
                : 0;

            foreach (var tidalAlbum in albumsWithoutUpc)
            {
                var albumResult = await _tidalIntegrator.GetAlbum(tidalAlbum.Id);

                if (albumResult == null)
                {
                    log.Add($"WARN Could not get album {tidalAlbum.Id} {tidalAlbum.Title}");
                }
                else
                {
                    var album = TidalDaoMapper.MapTidalAlbumModelToDao(albumResult);
                    TidalDbInserter.UpdateFields(_vaultContext, album, tidalAlbum);
                }

                log.Add($"Sleeping for {sleepTimeInSeconds} seconds");
                Thread.Sleep(sleepTimeInSeconds * 1000);
            }

            return log;
        }
    }
}