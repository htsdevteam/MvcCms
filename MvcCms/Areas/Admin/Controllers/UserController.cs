using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcCms.Areas.Admin.Controllers
{
    [RouteArea("admin")]
    [RoutePrefix("user")]
    public class UserController : Controller
    {
        // GET: Admin/User
        [Route("")]
        public ActionResult Index()
        {
            using (var manager = new Data.CmsUserManager())
            {
                var users = manager.Users.ToList();
                return View(users);
            }
        }
    }
}