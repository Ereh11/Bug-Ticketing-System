using BugTrackingSystem.DAL;
using FluentValidation;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BugTrackingSystem.BL;

public class CommentAddDtoValidator : AbstractValidator<CommentAddDto>
{
    private readonly IUnitWork _unitWork;
    
    public CommentAddDtoValidator(IUnitWork unitWork)
    {
        _unitWork = unitWork;
        
        RuleFor(x => x.Text)
            .NotEmpty()
            .WithMessage("Comment text is required.")
            .MaximumLength(2000)
            .WithMessage("Comment text cannot exceed 2000 characters.");
            
        RuleFor(x => x.BugId)
            .NotEmpty()
            .WithMessage("Bug ID is required.")
            .MustAsync(BeValidBugId)
            .WithMessage("Bug with the provided ID does not exist.");
            
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required.")
            .MustAsync(BeValidUserId)
            .WithMessage("User with the provided ID does not exist.");
    }
    
    private async Task<bool> BeValidBugId(Guid bugId, CancellationToken cancellationToken)
    {
        var bug = await _unitWork.BugRepository.GetByIdAsync(bugId);
        return bug != null;
    }
    
    private async Task<bool> BeValidUserId(Guid userId, CancellationToken cancellationToken)
    {
        var user = await _unitWork.UserRepository.GetByIdAsync(userId);
        return user != null;
    }
}