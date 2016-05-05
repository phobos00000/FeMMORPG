using System.Data.Entity.ModelConfiguration;

namespace FeMMORPG.Data
{
    public class ServerMap : EntityTypeConfiguration<Server>
    {
        public ServerMap()
        {
            HasKey(t => t.IP);

            Property(t => t.IP)
                .HasMaxLength(15)
                .IsRequired();

            Property(t => t.CurrentUsers)
                .IsRequired();

            Property(t => t.MaxUsers)
                .IsRequired();

            HasMany(t => t.LoginTokens)
                .WithRequired(t => t.Server)
                .HasForeignKey(t => t.ServerIP);
        }
    }
}
