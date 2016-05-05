namespace FeMMORPG.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
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
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            context.Servers.AddOrUpdate(
                p => p.IP,
                new Data.Server
                {
                    IP = "127.0.0.1",
                    CurrentUsers = 0,
                    MaxUsers = 100,
                    Enabled = true,
                }
                );

            context.Users.AddOrUpdate(
                p => p.Username,
                new User
                {
                    Username = "a",
                    Password = Convert.ToBase64String(new SHA256Managed().ComputeHash(Encoding.UTF8.GetBytes("a"))),
                    LastLogin = DateTime.Parse("2016-04-28 00:10:16"),
                    Enabled = true,
                }
                );
        }
    }
}
