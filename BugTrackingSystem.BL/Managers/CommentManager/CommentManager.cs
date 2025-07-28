using BugTrackingSystem.BL.Managers.CommentManager;
using BugTrackingSystem.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTrackingSystem.BL;

public class CommentManager : ICommentManager
{
    private readonly IUnitWork _unitWork;
    private readonly CommentAddDtoValidator _commentAddDtoValidator;
    
    public CommentManager(
        IUnitWork unitWork,
        CommentAddDtoValidator commentAddDtoValidator)
    {
        _unitWork = unitWork;
        _commentAddDtoValidator = commentAddDtoValidator;
    }
    
    public async Task<GeneralResult> AddCommentAsync(CommentAddDto commentDto)
    {
        var validationResult = await _commentAddDtoValidator.ValidateAsync(commentDto);
        if (!validationResult.IsValid)
        {
            return validationResult.MapErrorToGeneralResult();
        }
        
        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            Text = commentDto.Text,
            BugId = commentDto.BugId,
            UserId = commentDto.UserId,
            TextDate = commentDto.TextDate
        };
        
        await _unitWork.CommentRepository.AddAsync(comment);
        await _unitWork.SaveChangesAsync();
        
        return new GeneralResult
        {
            Success = true,
            Message = "Comment added successfully."
        };
    }
    
    public async Task<GeneralResult> DeleteCommentAsync(Guid commentId)
    {
        var comment = await _unitWork.CommentRepository.GetByIdAsync(commentId);
        if (comment == null)
        {
            return new GeneralResult
            {
                Success = false,
                Message = "Comment not found.",
                Errors = new List<ResultError>
                {
                    new ResultError
                    {
                        Message = "Comment not found.",
                        Code = "404"
                    }
                }
            };
        }
        
        await _unitWork.CommentRepository.DeleteAsync(commentId);
        await _unitWork.SaveChangesAsync();
        
        return new GeneralResult
        {
            Success = true,
            Message = "Comment deleted successfully."
        };
    }
    
    public async Task<GeneralResult> GetCommentByIdAsync(Guid commentId)
    {
        var comment = await _unitWork.CommentRepository.GetCommentWithBugAndUserAsync(commentId);
        if (comment == null)
        {
            return new GeneralResult
            {
                Success = false,
                Message = "Comment not found.",
                Errors = new List<ResultError>
                {
                    new ResultError
                    {
                        Message = "Comment not found.",
                        Code = "404"
                    }
                }
            };
        }
        
        var commentViewDto = new CommentViewDto
        {
            Text = comment.Text,
            UserName = $"{comment.User.FirstName} {comment.User.LastName}",
            TextDate = comment.TextDate,
        };
        
        return new GeneralResult<CommentViewDto>
        {
            Success = true,
            Message = "Comment retrieved successfully.",
            Data = commentViewDto
        };
    }
    
    public async Task<GeneralResult> GetCommentsByBugIdAsync(Guid bugId)
    {
        var bug = await _unitWork.BugRepository.GetByIdAsync(bugId);
        if (bug == null)
        {
            return new GeneralResult
            {
                Success = false,
                Message = "Bug not found.",
                Errors = new List<ResultError>
                {
                    new ResultError
                    {
                        Message = "Bug not found.",
                        Code = "404"
                    }
                }
            };
        }
        
        var comments = await _unitWork.CommentRepository.GetCommentsByBugIdAsync(bugId);
        if (!comments.Any())
        {
            return new GeneralResult<List<CommentViewDto>>
            {
                Success = true,
                Message = "No comments found for this bug.",
                Data = new List<CommentViewDto>()
            };
        }
        
        var commentViewDtos = comments.Select(c => new CommentViewDto
        {
            Text = c.Text,
            UserName = $"{c.User.FirstName} {c.User.LastName}",
            TextDate = c.TextDate,
        }).ToList();
        
        return new GeneralResult<List<CommentViewDto>>
        {
            Success = true,
            Message = "Comments retrieved successfully.",
            Data = commentViewDtos
        };
    }
    
    public async Task<GeneralResult> GetCommentsByUserIdAsync(Guid userId)
    {
        var user = await _unitWork.UserRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return new GeneralResult
            {
                Success = false,
                Message = "User not found.",
                Errors = new List<ResultError>
                {
                    new ResultError
                    {
                        Message = "User not found.",
                        Code = "404"
                    }
                }
            };
        }
        
        var comments = await _unitWork.CommentRepository.GetCommentsByUserIdAsync(userId);
        if (!comments.Any())
        {
            return new GeneralResult<List<CommentViewDto>>
            {
                Success = true,
                Message = "No comments found for this user.",
                Data = new List<CommentViewDto>()
            };
        }
        
        var commentViewDtos = comments.Select(c => new CommentViewDto
        {
            Text = c.Text,
            UserName = $"{user.FirstName} {user.LastName}",
            TextDate = c.TextDate,
        }).ToList();
        
        return new GeneralResult<List<CommentViewDto>>
        {
            Success = true,
            Message = "Comments retrieved successfully.",
            Data = commentViewDtos
        };
    }
}