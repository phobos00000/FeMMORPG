using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace FeMMORPG.Data
{
    public class UserMap : EntityTypeConfiguration<User>
    {
        public UserMap()
        {
            HasKey(t => t.Id);

            Property(t => t.Username)
                .HasMaxLength(128)
                .IsRequired()
                .HasColumnAnnotation(
                    "Index",
                    new IndexAnnotation(new IndexAttribute("IX_Username") { IsUnique = true }));

            Property(t => t.Password)
                .IsRequired();

            Property(t => t.LastLogin)
                .HasColumnType("datetime2");

            Property(t => t.Enabled)
                .IsRequired();

            HasMany(t => t.Characters)
                .WithRequired(t => t.User)
                .HasForeignKey(t => t.UserId);
        }
    }
}
