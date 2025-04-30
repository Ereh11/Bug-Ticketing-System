using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugTrackingSystem.DAL;

public interface IBugRepository : IGenericRepository<Bug>
{
    Task<List<Bug>?> GetBugWithProjectAndAttachmentInfo();
    Task<Bug?> GetBugWithProjectInfo(Guid bugId);
    Task<Bug?> GetBugWithProjectAndAttachmentInfo(Guid bugId);
}
