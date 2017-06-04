using MvcCms.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MvcCms.Data
{
    public interface IUserRepository : IDisposable
    {
        Task<CmsUser> GetUserByNameAsync(string username);
        Task<IEnumerable<CmsUser>> GetAllUsersAsync();
        Task CreateAsync(CmsUser user, string password);
        Task DeleteAsync(CmsUser user);
        Task UpdateAsync(CmsUser user);
        string HashPassword(string password);
        bool VerifyUserPassword(string hashedPassword, string providedPassword);
        Task AddUserToRoleAsync(CmsUser user, string role);
        Task<IEnumerable<string>> GetRolesForUserAsync(CmsUser user);
        Task RemoveUserFromRolesAsync(CmsUser user, params string[] roleNames);
        Task<CmsUser> GetLoginUserAsync(string userName, string password);
        Task<ClaimsIdentity> CreateIdentityAsync(CmsUser user);
    }
}