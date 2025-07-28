using BugTrackingSystem.DAL;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BugTrackingSystem.BL.Managers.CommentManager;

public interface ICommentManager
{
    Task<GeneralResult> AddCommentAsync(CommentAddDto commentDto);
    Task<GeneralResult> DeleteCommentAsync(Guid commentId);
    Task<GeneralResult> GetCommentByIdAsync(Guid commentId);
    Task<GeneralResult> GetCommentsByBugIdAsync(Guid bugId);
    Task<GeneralResult> GetCommentsByUserIdAsync(Guid userId);
}