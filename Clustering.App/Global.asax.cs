using Clustering.App;
using System.Web;
using System.Web.Http;

namespace Clustering
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            EntityConfig.InitializeDataContext();
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
