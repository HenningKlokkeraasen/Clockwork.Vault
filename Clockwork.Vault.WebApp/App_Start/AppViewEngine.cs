using System.Linq;
using System.Web.Mvc;

namespace Clockwork.Vault.WebApp
{
    public class AppViewEngine : RazorViewEngine
    {
        public AppViewEngine()
        {
            var newLocationFormat = new[]
            {
                "~/Views/Shared/Parts/{0}.cshtml",
            };

            PartialViewLocationFormats = PartialViewLocationFormats.Union(newLocationFormat).ToArray();
        }
    }
}