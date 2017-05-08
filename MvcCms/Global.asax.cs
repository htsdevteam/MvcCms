using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MvcCms.Models;
using MvcCms.Models.ModelBinders;
using MvcCms.App_Start;

namespace MvcCms
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            AuthDbConfig.RegisterAdmin();

            ModelBinders.Binders.Add(typeof(Post), new PostModelBinder());
        }
    }
}
