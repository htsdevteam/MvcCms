﻿using Microsoft.AspNet.Identity;
using MvcCms.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Security.Claims;
using System.Data.Entity;

namespace MvcCms.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly CmsUserStore _store;
        private readonly CmsUserManager _manager;

        private bool _disposed;


        public UserRepository()
        {
            _store = new CmsUserStore();
            _manager = new CmsUserManager(_store);
        }

        public async Task<CmsUser> GetUserByNameAsync(string username)
        {
            return await _store.FindByNameAsync(username);
        }

        public async Task<IEnumerable<CmsUser>> GetAllUsersAsync()
        {
            return await _store.Users.ToArrayAsync();
        }

        public async Task CreateAsync(CmsUser user, string password)
        {
            await _manager.CreateAsync(user, password);
        }

        public async Task DeleteAsync(CmsUser user)
        {
            await _manager.DeleteAsync(user);
        }

        public async Task UpdateAsync(CmsUser user)
        {
            await _manager.UpdateAsync(user);
        }

        public string HashPassword(string password)
        {
            return _manager.PasswordHasher.HashPassword(password);
        }

        public bool VerifyUserPassword(string hashedPassword, string providedPassword)
        {
            return _manager.PasswordHasher.VerifyHashedPassword(hashedPassword, providedPassword)
                == PasswordVerificationResult.Success;
        }

        public async Task AddUserToRoleAsync(CmsUser user, string role)
        {
            await _manager.AddToRoleAsync(user.Id, role);
        }

        public async Task<IEnumerable<string>> GetRolesForUserAsync(CmsUser user)
        {
            return await _manager.GetRolesAsync(user.Id);
        }

        public async Task RemoveUserFromRolesAsync(CmsUser user, params string[] roleNames)
        {
            await _manager.RemoveFromRolesAsync(user.Id, roleNames);
        }

        public async Task<CmsUser> GetLoginUserAsync(string userName, string password)
        {
            return await _manager.FindAsync(userName, password);
        }

        public async Task<ClaimsIdentity> CreateIdentityAsync(CmsUser user)
        {
            return await _manager.CreateIdentityAsync(user,
                DefaultAuthenticationTypes.ApplicationCookie);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _store.Dispose();
                _manager.Dispose();
            }
            _disposed = true;
        }
    }
}