using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MvcCms.Data
{
    public interface IRoleRepository : IDisposable
    {
        Task<IdentityRole> GetRoleByNameAsync(string name);
        IEnumerable<IdentityRole> GetAllRoles();
        Task CreateAsync(IdentityRole role);
    }
}