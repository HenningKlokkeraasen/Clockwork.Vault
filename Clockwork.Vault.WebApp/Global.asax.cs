using System.Web.Mvc;
using System.Web.Routing;

namespace Clockwork.Vault.WebApp
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            ViewEngines.Engines.Add(new AppViewEngine());
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}
