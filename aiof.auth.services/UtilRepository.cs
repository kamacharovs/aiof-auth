using System;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using aiof.auth.data;

namespace aiof.auth.services
{
    public class UtilRepository : IUtilRepository
    {
        private readonly ILogger<UtilRepository> _logger;
        private readonly AuthContext _context;

        public UtilRepository(
            ILogger<UtilRepository> logger,
            AuthContext context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        private IQueryable<Role> GetRolesQuery(bool asNoTracking = true)
        {
            return asNoTracking
                ? _context.Roles
                    .AsNoTracking()
                    .AsQueryable()
                : _context.Roles
                    .AsQueryable();
        }

        public async Task<IRole> GetRoleAsync<T>(
            int? id,
            bool asNoTracking = true)
            where T : IPublicKeyId
        {
            var defaultRole = string.Empty;
            
            switch (typeof(T).Name)
            {
                case nameof(User):
                    defaultRole = Roles.User;
                    break;
                case nameof(Client):
                    defaultRole = Roles.Client;
                    break;
                default:
                    defaultRole = Roles.User;
                    break;
            }

            defaultRole = defaultRole ?? throw new AuthFriendlyException(HttpStatusCode.BadRequest,
                $"Invalid request for a {nameof(Role)}. The currently supported entities are: {nameof(User)}, {nameof(Client)}");

            return await GetRolesQuery(asNoTracking)
                .FirstOrDefaultAsync(x => x.Id == id)
                ?? await GetRolesQuery(asNoTracking)
                    .FirstOrDefaultAsync(x => x.Name == defaultRole);
        }
    }
}