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


        public async Task<UserViewModel> GetUserByNameAsync(string name)
        {
            var user = await _userRepository.GetUserByNameAsync(name);
            if (user == null)
            {
                return null;
            }
            var vm = new UserViewModel
            {
                UserName = user.UserName,
                Email = user.Email,
                DisplayName = user.DisplayName
            };

            var userRoles = await _userRepository.GetRolesForUserAsync(user);
            vm.SelectedRole = userRoles.Count() > 1
                ? userRoles.FirstOrDefault()
                : userRoles.SingleOrDefault();
            vm.LoadUserRoles(await _roleRepository.GetAllRolesAsync());

            return vm;
        }

        public async Task<bool> CreateAsync(UserViewModel vm)
        {
            if (!_modelState.IsValid)
            {
                return false;
            }

            var existingUser = await _userRepository.GetUserByNameAsync(vm.UserName);
            if (existingUser != null)
            {
                _modelState.AddModelError("", "The user already exists!");
                return false;
            }

            if (string.IsNullOrWhiteSpace(vm.NewPassword))
            {
                _modelState.AddModelError("", "You must type a password.");
                return false;
            }

            var newUser = new CmsUser
            {
                UserName = vm.UserName,
                DisplayName = vm.DisplayName,
                Email = vm.Email
            };

            await _userRepository.CreateAsync(newUser, vm.NewPassword);
            await _userRepository.AddUserToRoleAsync(newUser, vm.SelectedRole);

            return true;
        }

        public async Task<bool> UpdateUser(UserViewModel vm)
        {
            var user = await _userRepository.GetUserByNameAsync(vm.UserName);
            if (user == null)
            {
                _modelState.AddModelError("", "The specified user does not exist.");
                return false;
            }

            if (!_modelState.IsValid)
            {
                return false;
            }

            if (!string.IsNullOrWhiteSpace(vm.NewPassword))
            {
                if (string.IsNullOrWhiteSpace(vm.CurrentPassword))
                {
                    _modelState.AddModelError("", "The current password must be supplied.");
                }

                var passwordVerified = _userRepository.VerifyUserPassword(
                    user.PasswordHash,
                    vm.CurrentPassword);

                if (!passwordVerified)
                {
                    _modelState.AddModelError("", "The current password does not match our records.");
                }

                var newHasedPassword = _userRepository.HashPassword(vm.NewPassword);
                user.PasswordHash = newHasedPassword;
            }

            user.Email = vm.Email;
            user.DisplayName = vm.DisplayName;

            await _userRepository.UpdateAsync(user);

            var roles = await _userRepository.GetRolesForUserAsync(user);
            await _userRepository.RemoveUserFromRolesAsync(user, roles.ToArray());
            await _userRepository.AddUserToRoleAsync(user, vm.SelectedRole);

            return true;
        }

        public async Task DeleteAsync(string username)
        {
            var user = await _userRepository.GetUserByNameAsync(username);
            if (user == null)
            {
                return;
            }
            await _userRepository.DeleteAsync(user);
        }

    }
}