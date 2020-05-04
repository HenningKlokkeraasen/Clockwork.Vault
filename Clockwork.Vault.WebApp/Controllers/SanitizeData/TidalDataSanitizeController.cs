using System.Linq;
using System.Web.Mvc;
using Clockwork.Vault.Core.Models;
using Clockwork.Vault.Query.Tidal;

namespace Clockwork.Vault.WebApp.Controllers.SanitizeData
{
    public class TidalDataSanitizeController : Controller
    {
        private readonly TidalOrchestrator _tidalOrchestrator;

        public TidalDataSanitizeController()
        {
            _tidalOrchestrator = new TidalOrchestrator();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FindDuplicateArtists()
        {
            var tidalArtists = _tidalOrchestrator.Artists;

            var groups = tidalArtists.GroupBy(a => a.Name);

            var log = new Log { Title = "Tidal: Duplicate artists" };

            foreach (var g in groups.Where(g => g.Count() > 1))
            {
                log.Statistics.Add($"{g.Key}: {g.Count()}");
            }

            return View("~/Views/Shared/Result.cshtml", log);
        }

        public ActionResult FindDuplicateAlbums()
        {
            var tidalArtists = _tidalOrchestrator.Albums;

            var groups = tidalArtists.GroupBy(a => a.Title);

            var log = new Log { Title = "Tidal: Duplicate albums" };

            foreach (var g in groups.Where(g => g.Count() > 1))
            {
                log.Statistics.Add($"{g.Key}: {g.Count()}");
            }

            return View("~/Views/Shared/Result.cshtml", log);
        }
    }
}