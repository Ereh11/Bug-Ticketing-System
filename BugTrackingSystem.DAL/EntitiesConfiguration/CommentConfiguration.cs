using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BugTrackingSystem.DAL;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.Text)
            .IsRequired()
            .HasMaxLength(2000);
            
        // Relationships
        builder
            .HasOne(c => c.Bug)
            .WithMany(b => b.Comments)
            .HasForeignKey(c => c.BugId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder
            .HasOne(c => c.User)
            .WithMany(u => u.Comments)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict); // Using Restrict to prevent user deletion cascading to comments
    }
}