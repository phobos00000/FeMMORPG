using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace FeMMORPG.Data
{
    public class CharacterMap : EntityTypeConfiguration<Character>
    {
        public CharacterMap()
        {
            HasKey(t => t.Id);

            Property(t => t.Name)
                .HasMaxLength(64)
                .IsRequired()
                .HasColumnAnnotation(
                    "Index",
                    new IndexAnnotation(new IndexAttribute("IX_Name") { IsUnique = true }));

            Property(t => t.Location.Map)
                .IsRequired();

            Property(t => t.Location.X)
                .IsRequired();

            Property(t => t.Location.Y)
                .IsRequired();

            Property(t => t.Location.Z)
                .IsRequired();
        }
    }
}
