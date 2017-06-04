using MvcCms.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MvcCms.Data
{
    public interface IUserRepository : IDisposable
    {
        Task<CmsUser> GetUserByNameAsync(string username);
        IEnumerable<CmsUser> GetAllUsers();
        Task CreateAsync(CmsUser user, string password);
        Task DeleteAsync(CmsUser user);
        Task UpdateAsync(CmsUser user);
        string HashPassword(string password);
        bool VerifyUserPassword(string hashedPassword, string providedPassword);
        Task AddUserToRoleAsync(CmsUser user, string role);
        Task<IEnumerable<string>> GetRolesForUserAsync(CmsUser user);
        Task RemoveUserFromRolesAsync(CmsUser user, params string[] roleNames);
    }
}