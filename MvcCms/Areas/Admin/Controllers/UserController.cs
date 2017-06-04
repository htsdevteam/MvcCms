using Microsoft.AspNet.Identity;
using MvcCms.Areas.Admin.Services;
using MvcCms.Areas.Admin.ViewModels;
using MvcCms.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MvcCms.Areas.Admin.Controllers
{
    [RouteArea("admin")]
    [RoutePrefix("user")]
    [Authorize]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly UserService _userService;
        private bool _isDisposed;

        public UserController()
        {
            _userRepository = new UserRepository();
            _roleRepository = new RoleRepository();
            _userService = new UserService(ModelState, _userRepository, _roleRepository);
        }

        // GET: Admin/User
        [Route("")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> Index()
        {
            var users = await _userRepository.GetAllUsersAsync();
            return View(users);
        }

        [Route("edit/{username}")]
        [Authorize(Roles = "admin, editor, author")]
        public async Task<ActionResult> Edit(string username)
        {
            var currentUser = User.Identity.Name;
            if (!User.IsInRole("admin")
                && !string.Equals(currentUser, username, StringComparison.CurrentCultureIgnoreCase))
            {
                return new HttpUnauthorizedResult();
            }

            var user = await _userService.GetUserByNameAsync(username);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [Route("edit/{username}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin, editor, author")]
        public async Task<ActionResult> Edit(UserViewModel vm, string username)
        {
            var currentUser = User.Identity.Name;
            var isAdmin = User.IsInRole("admin");
            if (!isAdmin &&
                !string.Equals(currentUser, username, StringComparison.CurrentCultureIgnoreCase))
            {
                return new HttpUnauthorizedResult();
            }

            var userUpdated = await _userService.UpdateUser(vm);
            if (userUpdated)
            {
                if (isAdmin)
                {
                    return RedirectToAction("index");
                }
                return RedirectToAction("index", "admin");
            }
            return View(vm);
        }

        [Route("delete/{username}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> Delete(string username)
        {
            await _userService.DeleteAsync(username);
            return RedirectToAction("index");
        }

        [Route("create")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> Create()
        {
            var vm = new UserViewModel();
            vm.LoadUserRoles(await _roleRepository.GetAllRolesAsync());
            return View(vm);
        }

        [Route("create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> Create(UserViewModel vm)
        {
            var completed = await _userService.CreateAsync(vm);
            if (completed)
            {
                return RedirectToAction("index");
            }
            return View(vm);
        }

        protected override void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                _userRepository.Dispose();
                _roleRepository.Dispose();
            }
            _isDisposed = true;
            base.Dispose(disposing);
        }
    }
}