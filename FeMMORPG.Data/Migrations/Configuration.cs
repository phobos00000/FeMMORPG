namespace FeMMORPG.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    internal sealed class Configuration : DbMigrationsConfiguration<FeMMORPG.Data.UserDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(FeMMORPG.Data.UserDbContext context)
        {
            context.Users.AddOrUpdate(
                p => p.Username,
                new User
                {
                    Username = "a",
                    Password = Convert.ToBase64String(new SHA256Managed().ComputeHash(Encoding.UTF8.GetBytes("a"))),
                    LastLogin = DateTime.Parse("2016-04-28 00:10:16"),
                    Enabled = true,
                });

            var user = context.Users
                .Where(u => u.Username == "a")
                .FirstOrDefault();

            context.Characters.AddOrUpdate(
                p => p.Name,
                new Character
                {
                    Name = "Jolly",
                    Surname = "Roger",
                    UserId = user.Id,
                });
        }
    }
}
