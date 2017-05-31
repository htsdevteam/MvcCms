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
                var user = userRepository.GetUserByName("admin");

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
                if (roleRepository.GetRoleByName("admin") == null)
                {
                    roleRepository.Create(new IdentityRole("admin"));
                }
                if (roleRepository.GetRoleByName("editor") == null)
                {
                    roleRepository.Create(new IdentityRole("editor"));
                }
                if (roleRepository.GetRoleByName("author") == null)
                {
                    roleRepository.Create(new IdentityRole("author"));
                }
            }
        }
    }
}