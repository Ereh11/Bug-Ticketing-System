using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTrackingSystem.DAL;

public class CommentRepository : GenericRepository<Comment>, ICommentRepository
{
    private readonly BugTrackingSystemContext _context;
    
    public CommentRepository(BugTrackingSystemContext context)
        : base(context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Comment>> GetCommentsByBugIdAsync(Guid bugId)
    {
        return await _context.Set<Comment>()
            .Where(c => c.BugId == bugId)
            .Include(c => c.User)
            .AsNoTracking()
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Comment>> GetCommentsByUserIdAsync(Guid userId)
    {
        return await _context.Set<Comment>()
            .Where(c => c.UserId == userId)
            .Include(c => c.Bug)
            .AsNoTracking()
            .ToListAsync();
    }
    
    public async Task<Comment?> GetCommentWithBugAndUserAsync(Guid commentId)
    {
        return await _context.Set<Comment>()
            .Include(c => c.Bug)
            .Include(c => c.User)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == commentId);
    }
}