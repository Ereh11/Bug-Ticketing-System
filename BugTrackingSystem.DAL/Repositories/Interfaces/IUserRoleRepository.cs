using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugTrackingSystem.DAL;

public interface IUserRoleRepository : IGenericRepository<UserRole>
{
    Task RemoveUserRoleAsync(Guid userId, Guid roleId);
    Task<bool> ExistsByCompositeKeyAsync(Guid userId, Guid roleId);
}
