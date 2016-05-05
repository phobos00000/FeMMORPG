namespace FeMMORPG.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Servers",
                c => new
                {
                    IP = c.String(nullable: false, maxLength: 15),
                    CurrentUsers = c.Int(nullable: false),
                    MaxUsers = c.Int(nullable: false),
                    Enabled = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.IP);

            CreateTable(
                "dbo.LoginTokens",
                c => new
                {
                    Id = c.Guid(nullable: false, defaultValueSql: "newsequentialid()"),
                    LoginTime = c.DateTime(nullable: false),
                    ServerIP = c.String(nullable: false, maxLength: 15),
                    UserId = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId)
                .ForeignKey("dbo.Servers", t => t.ServerIP, cascadeDelete: true)
                .Index(t => t.ServerIP)
                .Index(t => t.UserId);

            CreateTable(
                "dbo.Users",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Username = c.String(nullable: false, maxLength: 128),
                    Password = c.String(nullable: false),
                    LastLogin = c.DateTime(precision: 7, storeType: "datetime2"),
                    Enabled = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Username, unique: true);

        }

        public override void Down()
        {
            DropForeignKey("dbo.LoginTokens", "ServerIP", "dbo.Servers");
            DropForeignKey("dbo.LoginTokens", "UserId", "dbo.Users");
            DropIndex("dbo.Users", new[] { "Username" });
            DropIndex("dbo.LoginTokens", new[] { "UserId" });
            DropIndex("dbo.LoginTokens", new[] { "ServerIP" });
            DropTable("dbo.Users");
            DropTable("dbo.LoginTokens");
            DropTable("dbo.Servers");
        }
    }
}
