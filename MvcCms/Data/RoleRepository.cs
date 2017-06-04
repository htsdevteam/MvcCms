using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcCms.Data
{
    public class RoleRepository : IRoleRepository
    {
        private readonly RoleStore<IdentityRole> _store;
        private readonly RoleManager<IdentityRole> _manager;

        private bool _disposed;


        public RoleRepository()
        {
            _store = new RoleStore<IdentityRole>();
            _manager = new RoleManager<IdentityRole>(_store);
        }

        public async Task<IdentityRole> GetRoleByNameAsync(string name)
        {
            return await _store.FindByNameAsync(name);
        }

        public IEnumerable<IdentityRole> GetAllRoles()
        {
            return _store.Roles.ToArray();
        }

        public async Task CreateAsync(IdentityRole role)
        {
            await _manager.CreateAsync(role);
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