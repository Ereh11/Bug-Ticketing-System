using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugTrackingSystem.DAL;

public class BugRepository : GenericRepository<Bug>, IBugRepository
{
    private readonly BugTrackingSystemContext _context;
    public BugRepository(BugTrackingSystemContext context)
        : base(context)
    {
        _context = context;
    }
    public async Task<List<Bug>?> GetBugWithProjectAndAttachmentInfo()
    {
        return await _context.Set<Bug>()
            .Include(b => b.Project)
            .Include(b => b.Attachments)
            .AsNoTracking()
            .ToListAsync();
    }
    public async Task<Bug?> GetBugWithProjectAndAttachmentInfo(Guid bugId)
    {
        return await _context.Set<Bug>()
            .Include(b => b.Project)
            .Include(b => b.Attachments)
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.BugId == bugId);
    }
    public async Task<Bug?> GetBugWithProjectInfo(Guid bugId)
    {
        return await _context.Set<Bug>()
            .Include(b => b.Project)
            .FirstOrDefaultAsync(b => b.BugId == bugId);
    }
}
