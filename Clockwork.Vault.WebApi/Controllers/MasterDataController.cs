using System.Web.Http;
using Clockwork.Vault.Query.Master;

namespace Clockwork.Vault.WebApi.Controllers
{
    public class MasterDataController : ApiController
    {
        private readonly MasterDataOrchestrator _masterDataOrchestrator;

        internal MasterDataController()
        {
            _masterDataOrchestrator = new MasterDataOrchestrator();
        }

        [HttpGet]
        public IHttpActionResult Artists() => Ok(_masterDataOrchestrator.Artists);
    }
}
