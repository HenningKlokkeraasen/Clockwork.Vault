using System.Web.Http;
using Clockwork.Vault.Integrations.Tidal.Orchestration;

namespace Clockwork.Vault.WebApi.Controllers
{
    public class TidalController : ApiController
    {
        private readonly TidalOrchestrator _tidalOrchestrator;

        public TidalController()
        {
            _tidalOrchestrator = new TidalOrchestrator();
        }

        [HttpGet]
        public IHttpActionResult Artists() => Ok(_tidalOrchestrator.Artists);

        [HttpGet]
        public IHttpActionResult Albums() => Ok(_tidalOrchestrator.Albums);

        [HttpGet]
        public IHttpActionResult Playlists() => Ok(_tidalOrchestrator.Playlists);

        [HttpGet]
        public IHttpActionResult Tracks() => Ok(_tidalOrchestrator.Tracks);

        [HttpGet]
        public IHttpActionResult Artists(int id)
        {
            var artist = _tidalOrchestrator.GetArtist(id);

            if (artist != null)
                return Ok(artist);

            return NotFound();
        }

        [HttpGet]
        public IHttpActionResult Albums(int id, [FromUri]bool expand)
        {
            if (expand)
            {
                var album = _tidalOrchestrator.GetAlbumExpanded(id);

                if (album != null)
                    return Ok(album);
            }
            else
            {
                var album = _tidalOrchestrator.GetAlbum(id);

                if (album != null)
                    return Ok(album);
            }

            return NotFound();
        }

        [HttpGet]
        public IHttpActionResult Playlists(string id, [FromUri]bool expand = false)
        {
            if (expand)
            {
                var playlist = _tidalOrchestrator.GetPlaylistExpanded(id);

                if (playlist != null)
                    return Ok(playlist);
            }
            else
            {
                var playlist = _tidalOrchestrator.GetPlaylist(id);

                if (playlist != null)
                    return Ok(playlist);
            }

            return NotFound();
        }


        [HttpGet]
        public IHttpActionResult Tracks(int id, [FromUri]bool expand = false)
        {
            if (expand)
            {
                var track = _tidalOrchestrator.GetTrackExpanded(id);

                if (track != null)
                    return Ok(track);
            }
            else
            {
                var track = _tidalOrchestrator.GetTrack(id);

                if (track != null)
                    return Ok(track);
            }

            return NotFound();
        }
    }
}