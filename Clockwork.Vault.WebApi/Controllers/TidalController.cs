using System.Web.Http;
using Clockwork.Vault.Integrations.Tidal.Orchestration;

namespace Clockwork.Vault.WebApi.Controllers
{
    public class TidalController : ApiController
    {
        private readonly TidalRepository _tidalRepository;

        public TidalController()
        {
            _tidalRepository = new TidalRepository();
        }

        [HttpGet]
        public IHttpActionResult Artists() => Ok(_tidalRepository.Artists);

        [HttpGet]
        public IHttpActionResult Albums() => Ok(_tidalRepository.Albums);

        [HttpGet]
        public IHttpActionResult Playlists() => Ok(_tidalRepository.Playlists);

        [HttpGet]
        public IHttpActionResult Tracks() => Ok(_tidalRepository.Tracks);

        [HttpGet]
        public IHttpActionResult Artists(int id)
        {
            var artist = _tidalRepository.GetArtist(id);

            if (artist != null)
                return Ok(artist);

            return NotFound();
        }

        [HttpGet]
        public IHttpActionResult Albums(int id)
        {
            var album = _tidalRepository.GetAlbum(id);

            if (album != null)
                return Ok(album);

            return NotFound();
        }

        [HttpGet]
        public IHttpActionResult Playlists(string id)
        {
            var playlist = _tidalRepository.GetPlaylist(id);

            if (playlist != null)
                return Ok(playlist);

            return NotFound();
        }

        [HttpGet]
        public IHttpActionResult Tracks(int id)
        {
            var track = _tidalRepository.GetTrack(id);

            if (track != null)
                return Ok(track);

            return NotFound();
        }
    }
}