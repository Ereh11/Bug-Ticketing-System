using Microsoft.AspNetCore.Identity;

namespace BugTrackingSystem.DAL
{
    public class User : IdentityUser<Guid>
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public ICollection<UserRole> UserRoles { get; set; } = new HashSet<UserRole>();
        public ICollection<BugAssignment> BugAssignments { get; set; } = new HashSet<BugAssignment>();
        public ICollection<ProjectMember> ProjectMembers { get; set; } = new HashSet<ProjectMember>();
        public ICollection<Project> ManagedProjects { get; set; } = new HashSet<Project>();
        public ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
    }
}
