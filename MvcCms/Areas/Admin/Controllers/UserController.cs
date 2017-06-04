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
        public ActionResult Index()
        {
            var users = _userRepository.GetAllUsers();
            return View(users);
        }

        [Route("edit/{username}")]
        public async Task<ActionResult> Edit(string username)
        {
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
        public async Task<ActionResult> Edit(UserViewModel vm)
        {
            var userUpdated = await _userService.UpdateUser(vm);
            if (userUpdated)
            {
                return RedirectToAction("index");
            }
            return View(vm);


            using (var userStore = new CmsUserStore())
            using (var userManager = new CmsUserManager(userStore))
            {
                var user = userStore.FindByNameAsync(vm.UserName).Result;
                if (user == null)
                {
                    return HttpNotFound();
                }

                if (!ModelState.IsValid)
                {
                    return View(vm);
                }
                if (!string.IsNullOrWhiteSpace(vm.NewPassword))
                {
                    if (string.IsNullOrWhiteSpace(vm.CurrentPassword))
                    {
                        ModelState.AddModelError(string.Empty,
                            "The current password must be supplied.");
                        return View(vm);
                    }

                    var passwordVerified
                        = userManager.PasswordHasher.VerifyHashedPassword(
                            user.PasswordHash, vm.CurrentPassword);
                    if (passwordVerified != PasswordVerificationResult.Success)
                    {
                        ModelState.AddModelError(string.Empty,
                            "The current password must be supplied.");
                        return View(vm);
                    }

                    var newHashedPassword = userManager.PasswordHasher.HashPassword(
                        vm.NewPassword);
                    user.PasswordHash = newHashedPassword;
                }

                user.Email = vm.Email;
                user.DisplayName = vm.DisplayName;

                var updateResult = userManager.UpdateAsync(user).Result;
                if (updateResult.Succeeded)
                {
                    return RedirectToAction("index");
                }

                ModelState.AddModelError(string.Empty,
                    "An error occurred. Please try again.");
                return View(vm);
            }
        }

        [Route("delete/{username}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string username)
        {
            await _userService.DeleteAsync(username);
            return RedirectToAction("index");


            using (var userStore = new CmsUserStore())
            using (var userManager = new CmsUserManager(userStore))
            {
                var user = userStore.FindByNameAsync(username).Result;
                if (user == null)
                {
                    return HttpNotFound();
                }
                var deleteResult = userManager.DeleteAsync(user).Result;

                return RedirectToAction("index");
            }
        }

        [Route("create")]
        public ActionResult Create()
        {
            var vm = new UserViewModel();
            vm.LoadUserRoles(_roleRepository.GetAllRoles());
            return View(vm);
        }

        [Route("create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
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