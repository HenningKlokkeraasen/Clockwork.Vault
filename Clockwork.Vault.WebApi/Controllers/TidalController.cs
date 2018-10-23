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
    }
}