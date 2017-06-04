using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MvcCms.Data;
using MvcCms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MvcCms.App_Start
{
    public class AuthDbConfig
    {
        public async static Task RegisterAdmin()
        {
            using (var userRepository = new UserRepository())
            {
                var user = userRepository.GetUserByNameAsync("admin");

                if (user == null)
                {
                    var adminUser = new CmsUser
                    {
                        UserName = "admin",
                        Email = "admin@cms.com",
                        DisplayName = "Administrator"
                    };
                    await userRepository.CreateAsync(adminUser, "Passw0rd1234");
                }
            }

            using (var roleRepository = new RoleRepository())
            {
                if (roleRepository.GetRoleByNameAsync("admin") == null)
                {
                    await roleRepository.CreateAsync(new IdentityRole("admin"));
                }
                if (roleRepository.GetRoleByNameAsync("editor") == null)
                {
                    await roleRepository.CreateAsync(new IdentityRole("editor"));
                }
                if (roleRepository.GetRoleByNameAsync("author") == null)
                {
                    await roleRepository.CreateAsync(new IdentityRole("author"));
                }
            }
        }
    }
}