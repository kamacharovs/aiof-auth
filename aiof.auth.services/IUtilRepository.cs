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
            bool asNoTracking = true)
            where T : IPublicKeyId;
    }
}