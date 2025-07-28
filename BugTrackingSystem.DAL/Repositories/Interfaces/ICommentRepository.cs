using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BugTrackingSystem.DAL;

public interface ICommentRepository : IGenericRepository<Comment>
{
    Task<IEnumerable<Comment>> GetCommentsByBugIdAsync(Guid bugId);
    Task<IEnumerable<Comment>> GetCommentsByUserIdAsync(Guid userId);
    Task<Comment?> GetCommentWithBugAndUserAsync(Guid commentId);
}