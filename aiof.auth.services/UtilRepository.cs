using System;
using System.Net;
using System.Linq;
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

        private IQueryable<Role> GetRolesQuery(bool asNoTracking = false)
        {
            return asNoTracking
                ? _context.Roles
                    .AsNoTracking()
                    .AsQueryable()
                : _context.Roles
                    .AsQueryable();
        }

        /// <summary>
        /// Get a Role by id. If the id is null, then a default role is assigned and retrieved from the database.
        /// If the Role still doesn't exist in the database, then it's added. The default roles are assigned
        /// based on T name (User, Client, etc.)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="asNoTracking"></param>
        /// <returns></returns>
        public async Task<IRole> GetRoleAsync<T>(
            int? id,
            bool asNoTracking = false)
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
                    defaultRole = Roles.Basic;
                    break;
            }

            defaultRole = defaultRole ?? throw new AuthFriendlyException(HttpStatusCode.BadRequest,
                $"Invalid request for a {nameof(Role)}. The currently supported entities are: {nameof(User)}, {nameof(Client)}");

            return await GetRolesQuery(asNoTracking)
                .FirstOrDefaultAsync(x => x.Id == id)
                ?? await GetRolesQuery(asNoTracking)
                    .FirstOrDefaultAsync(x => x.Name == defaultRole)
                    ?? await QuickAddRoleAsync(defaultRole);
        }

        /// <summary>
        /// Get default Role
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="asNoTracking"></param>
        /// <returns></returns>
        public async Task<int> GetRoleIdAsync<T>(bool asNoTracking = true) where T : IPublicKeyId
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
                    defaultRole = Roles.Basic;
                    break;
            }

            var role = await GetRolesQuery(asNoTracking)
                .FirstOrDefaultAsync(x => x.Name == defaultRole)
                ?? throw new AuthFriendlyException(HttpStatusCode.BadRequest,
                    $"Default role for Entity={typeof(T).Name}) was not found");

            return role.Id;
        }

        /// <summary>
        /// Add Role in a quick and invalidated way
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<IRole> QuickAddRoleAsync(
            string name)
        {
            var role = new Role { Name = name };
            
            await _context.Roles.AddAsync(role);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Created {nameof(Role)}='{name}'");

            return role;
        }
    }
}