﻿using MvcCms.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MvcCms.Data
{
    public interface IUserRepository : IDisposable
    {
        CmsUser GetUserByName(string username);
        IEnumerable<CmsUser> GetAllUsers();
        Task CreateAsync(CmsUser user, string password);
        void Delete(CmsUser user);
        void Update(CmsUser user);
    }
}