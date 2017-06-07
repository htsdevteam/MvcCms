using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MvcCms.Data;
using MvcCms.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MvcCms.App_Start
{
    public class AuthDbConfig
    {
        public async static Task RegisterAdmin()
        {
            CmsUser admin = null;
            using (var userRepository = new UserRepository())
            {
                admin = await userRepository.GetUserByNameAsync("admin");

                if (admin == null)
                {
                    admin = new CmsUser
                    {
                        UserName = "admin",
                        Email = "admin@cms.com",
                        DisplayName = "Administrator"
                    };
                    await userRepository.CreateAsync(admin, "Passw0rd1234");
                }
            }

            using (var roleRepository = new RoleRepository())
            {
                IdentityRole adminRole = await roleRepository.GetRoleByNameAsync("admin");
                if (adminRole == null)
                {
                    await roleRepository.CreateAsync(new IdentityRole("admin"));
                }
                bool isAdminInAdminRole = false;
                foreach (IdentityUserRole userRole in admin.Roles)
                {
                    if (userRole.RoleId == adminRole.Id)
                    {
                        isAdminInAdminRole = true;
                        break;
                    }
                }
                if (!isAdminInAdminRole)
                {
                    using (var userRepository = new UserRepository())
                    {
                        await userRepository.AddUserToRoleAsync(admin, "admin");
                    }
                }

                if (await roleRepository.GetRoleByNameAsync("editor") == null)
                {
                    await roleRepository.CreateAsync(new IdentityRole("editor"));
                }
                if (await roleRepository.GetRoleByNameAsync("author") == null)
                {
                    await roleRepository.CreateAsync(new IdentityRole("author"));
                }
            }
        }
    }
}