using Microsoft.Owin.Security;
using MvcCms.Areas.Admin.ViewModels;
using MvcCms.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MvcCms.Areas.Admin.Controllers
{
    [RouteArea("admin")]
    [Authorize]
    public class AdminController : Controller
    {
        private readonly IUserRepository _userRepository;
        private bool _isDisposed;

        public AdminController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public AdminController() : this(new UserRepository())
        { }


        // GET: Admin/Admin
        [Route("")]
        public ActionResult Index()
        {
            return View();
        }

        [Route("login")]
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<ActionResult> Login(LoginViewModel vm)
        {
            var user = await _userRepository.GetLoginUserAsync(vm.UserName, vm.Password);
            if (user == null)
            {
                ModelState.AddModelError("", "The user with the supplied credentials does not exists.");
                return View(vm);
            }

            var authManager = HttpContext.GetOwinContext().Authentication;
            var userIdentity = await _userRepository.CreateIdentityAsync(user);

            authManager.SignIn(new AuthenticationProperties { IsPersistent = vm.RememberMe },
                userIdentity);

            return RedirectToAction("index");
        }

        [Route("logout")]
        public ActionResult Logout()
        {
            var authManager = HttpContext.GetOwinContext().Authentication;
            authManager.SignOut();
            return RedirectToAction("index", "home");
        }

        [AllowAnonymous]
        public PartialViewResult AdminMenu()
        {
            var items = new List<AdminMenuItem>();
            if (User.Identity.IsAuthenticated)
            {
                items.Add(new AdminMenuItem
                {
                    Text = "Admin home",
                    Action = "index",
                    RouteInfo = new { controller = "admin", area = "admin" }
                });

                if (User.IsInRole("admin"))
                {
                    items.Add(new AdminMenuItem
                    {
                        Text = "Users",
                        Action = "index",
                        RouteInfo = new { controller = "user", area = "admin" }
                    });
                }
                else
                {
                    items.Add(new AdminMenuItem
                    {
                        Text = "Profile",
                        Action = "edit",
                        RouteInfo = new { controller = "user", area = "admin", username = User.Identity.Name }
                    });
                }

                if (!User.IsInRole("author"))
                {
                    items.Add(new AdminMenuItem
                    {
                        Text = "Tags",
                        Action = "index",
                        RouteInfo = new { controller = "tag", area = "admin" }
                    });
                }

                items.Add(new AdminMenuItem
                {
                    Text = "Posts",
                    Action = "index",
                    RouteInfo = new { controller = "post", area = "admin" }
                });
            }
            return PartialView(items);
        }

        protected override void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                _userRepository.Dispose();
            }
            _isDisposed = true;
            base.Dispose(disposing);
        }
    }
}