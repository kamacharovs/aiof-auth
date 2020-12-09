using System;
using System.Security.Claims;
using System.Collections.Generic;
using System.Threading.Tasks;

using aiof.auth.data;

namespace aiof.auth.services
{
    public interface IUtilRepository
    {
        Task<IRole> GetRoleAsync<T>(
            int? id,
            bool asNoTracking = false)
            where T : IPublicKeyId;
        Task<int> GetRoleIdAsync<T>(bool asNoTracking = false) where T : IPublicKeyId;
        Task<IRole> QuickAddRoleAsync(string name);
    }
}