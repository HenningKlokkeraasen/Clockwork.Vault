using System.Web.Mvc;
using Clockwork.Vault.Query.Tidal;

namespace Clockwork.Vault.WebApp.Controllers
{
    public class TidalController : Controller
    {
        private readonly TidalOrchestrator _tidalOrchestrator;

        public TidalController()
        {
            _tidalOrchestrator = new TidalOrchestrator();
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Artists(int? id)
        {
            if (id.HasValue)
            {
                var artist = _tidalOrchestrator.GetArtistExpanded(id.Value);

                if (artist != null)
                    return View("~/Views/Tidal/Artist.cshtml", artist);

                return View("~/Views/Shared/NotFound.cshtml");
            }

            return View(_tidalOrchestrator.GetArtistsOrderedByNameGroupedByIsFavorite());
        }

        [HttpGet]
        public ActionResult Albums(int? id)
        {
            if (id.HasValue)
            {
                var album = _tidalOrchestrator.GetAlbumExpanded(id.Value);

                if (album != null)
                    return View("~/Views/Tidal/Album.cshtml", album);
                
                return View("~/Views/Shared/NotFound.cshtml");
            }

            return View(_tidalOrchestrator.GetAlbumsOrderedByTitleGroupedByIsFavorite());
        }

        [HttpGet]
        public ActionResult Tracks(int? id)
        {
            if (id.HasValue)
            {
                var track = _tidalOrchestrator.GetTrackExpanded(id.Value);

                if (track != null)
                    return View("~/Views/Tidal/Track.cshtml", track);

                return View("~/Views/Shared/NotFound.cshtml");
            }

            return View(_tidalOrchestrator.GetTracksOrderedByTitleGroupedByIsFavorite());
        }

        [HttpGet]
        public ActionResult Playlists(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var playlist = _tidalOrchestrator.GetPlaylistExpanded(id);

                if (playlist != null)
                    return View("~/Views/Tidal/Playlist.cshtml", playlist);

                return View("~/Views/Shared/NotFound.cshtml");
            }

            return View(_tidalOrchestrator.GetPlaylistsGroupedByCreator());
        }
    }
}