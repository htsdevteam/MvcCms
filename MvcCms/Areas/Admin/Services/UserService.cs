using MvcCms.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcCms.Areas.Admin.ViewModels;
using MvcCms.Models;
using System.Threading.Tasks;

namespace MvcCms.Areas.Admin.Services
{
    public class UserService
    {
        private readonly ModelStateDictionary _modelState;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;

        public UserService(ModelStateDictionary modelState,
            IUserRepository userRepository,
            IRoleRepository roleRepository)
        {
            _modelState = modelState;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        public async Task<bool> CreateAsync(UserViewModel vm)
        {
            if (!_modelState.IsValid)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(vm.NewPassword))
            {
                _modelState.AddModelError(string.Empty, "You must type a password.");
                return true;
            }

            var newUser = new CmsUser
            {
                UserName = vm.UserName,
                DisplayName = vm.DisplayName,
                Email = vm.Email
            };

            await _userRepository.CreateAsync(newUser, vm.NewPassword);

            return true;
        }
    }
}