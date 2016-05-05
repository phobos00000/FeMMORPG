using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace FeMMORPG.Data.Mappings
{
    public class LoginTokenMap : EntityTypeConfiguration<LoginToken>
    {
        public LoginTokenMap()
        {
            HasKey(t => t.Id);

            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.LoginTime)
                .IsRequired();

            Property(t => t.ServerIP)
                .IsRequired();

            HasRequired(t => t.User)
                .WithOptional(t => t.LoginToken);
        }
    }
}
