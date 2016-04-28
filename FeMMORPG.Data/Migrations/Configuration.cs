namespace FeMMORPG.Data.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    using System.Security.Cryptography;
    using System.Text;
    internal sealed class Configuration : DbMigrationsConfiguration<FeMMORPG.Data.UserContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(FeMMORPG.Data.UserContext context)
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

            context.Users.AddOrUpdate(
                p => p.Username,
                new User
                {
                    Username = "a",
                    Password = Convert.ToBase64String(new SHA256Managed().ComputeHash(Encoding.UTF8.GetBytes("a"))),
                    LastLogin = DateTime.Parse("2016-04-28 00:10:16"),
                    Enabled = true,
                    Characters = new List<Character>
                    {
                        new Character
                        {
                            Name = "Johnny",
                            Location = new Location
                            {
                                Map = "home",
                                X = 1,
                                Y = 2,
                                Z = 3,
                            },
                        }
                    }
                }
                );
        }
    }
}
