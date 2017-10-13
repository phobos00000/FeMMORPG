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

            Property(t => t.LastLogout)
                .HasColumnType("datetime2");

            Property(t => t.SecondsPlayed);

            Property(t => t.Enabled)
                .IsRequired();

            HasOptional(t => t.LoginToken)
                .WithRequired(t => t.User)
                .Map(m => m.MapKey("UserId"));

            HasMany(t => t.Characters)
                .WithRequired(t => t.User)
                .HasForeignKey(t => t.UserId);
        }
    }
}
