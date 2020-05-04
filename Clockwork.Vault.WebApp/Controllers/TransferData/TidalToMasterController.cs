using System.Web.Mvc;
using Clockwork.Vault.Dao;
using Clockwork.Vault.DataTransfer.TidalToMaster;

namespace Clockwork.Vault.WebApp.Controllers.TransferData
{
    public class TidalToMasterController : Controller
    {
        private VaultContext _vaultContext;

        private TidalToMasterDataOrchestrator Orchestrator => new TidalToMasterDataOrchestrator(_vaultContext);

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TransferArtists()
        {
            GetInMemContextOrEstablish();
            var vm = Orchestrator.TransferArtists();
            return View("~/Views/Shared/Result.cshtml", vm);
        }

        public ActionResult TransferAlbums()
        {
            GetInMemContextOrEstablish();
            var vm = Orchestrator.TransferAlbums();
            return View("~/Views/Shared/Result.cshtml", vm);
        }

        public ActionResult TransferTracks()
        {
            GetInMemContextOrEstablish();
            var vm = Orchestrator.TransferTracks();
            return View("~/Views/Shared/Result.cshtml", vm);
        }

        public ActionResult TransferPlaylists()
        {
            GetInMemContextOrEstablish();
            var vm = Orchestrator.TransferPlaylists();
            return View("~/Views/Shared/Result.cshtml", vm);
        }

        public ActionResult TransferAlbumArtists()
        {
            GetInMemContextOrEstablish();
            var vm = Orchestrator.TransferAlbumArtists();
            return View("~/Views/Shared/Result.cshtml", vm);
        }

        private void GetInMemContextOrEstablish() => _vaultContext = new VaultContext();
    }
}