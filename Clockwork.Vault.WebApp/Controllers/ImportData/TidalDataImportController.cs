using System.Configuration;
using System.Threading.Tasks;
using System.Web.Mvc;
using Clockwork.Vault.Integrations.Tidal.Orchestration;

namespace Clockwork.Vault.WebApp.Controllers.ImportData
{
    public class TidalDataImportController : Controller
    {
        private SaveTidalDataOrchestrator _orchestrator;

        public TidalDataImportController()
        {
            var appSettingsReader = new AppSettingsReader();
            var token = appSettingsReader.GetValue("tidal.token", typeof(string)) as string;
            var username = appSettingsReader.GetValue("tidal.username", typeof(string)) as string;
            var password = appSettingsReader.GetValue("tidal.password", typeof(string)) as string;
            _orchestrator = new SaveTidalDataOrchestrator(token, username, password);
        }

        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> SavePlaylists()
        {
            var result = await _orchestrator.SavePlaylists();
            return View("~/Views/Shared/Result.cshtml", result);
        }

        public async Task<ActionResult> SaveUserFavPlaylists()
        {
            var result = await _orchestrator.SaveUserFavPlaylists();
            return View("~/Views/Shared/Result.cshtml", result);
        }

        public async Task<ActionResult> SaveUserFavAlbums()
        {
            var result = await _orchestrator.SaveUserFavAlbums();
            return View("~/Views/Shared/Result.cshtml", result);
        }

        public async Task<ActionResult> SaveUserFavTracks()
        {
            var result = await _orchestrator.SaveUserFavTracks();
            return View("~/Views/Shared/Result.cshtml", result);
        }

        public async Task<ActionResult> SaveUserFavArtists()
        {
            var result = await _orchestrator.SaveUserFavArtists();
            return View("~/Views/Shared/Result.cshtml", result);
        }

        public async Task<ActionResult> EnsureAlbumUpc()
        {
            var iterationSettings = new IterationSettings
            {
                SleepTimeInSeconds = 1
            };
            var result = await _orchestrator.EnsureAlbumUpc(iterationSettings);
            return View("~/Views/Shared/Result.cshtml", result);
        }

        public async Task<ActionResult> EnsureTrackIsrc()
        {
            var iterationSettings = new IterationSettings
            {
                SleepTimeInSeconds = 1
            };
            var result = await _orchestrator.EnsureTrackIsrc(iterationSettings);
            return View("~/Views/Shared/Result.cshtml", result);
        }
    }
}