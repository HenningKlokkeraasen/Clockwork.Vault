using System.Linq;
using System.Web.Mvc;
using Clockwork.Vault.Core.Models;
using Clockwork.Vault.Query.Tidal;

namespace Clockwork.Vault.WebApp.Controllers
{
    public class TidalController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FindDuplicateArtists()
        {
            var tidalArtists = new TidalOrchestrator().Artists;

            var groups = tidalArtists.GroupBy(a => a.Name);

            var log = new Log {Title = "Tidal: Duplicate artists"};

            foreach (var g in groups.Where(g => g.Count() > 1))
            {
                log.Statistics.Add($"{g.Key}: {g.Count()}");
            }

            return View("~/Views/Shared/Result.cshtml", log);
        }

        public ActionResult FindDuplicateAlbums()
        {
            var tidalArtists = new TidalOrchestrator().Albums;

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