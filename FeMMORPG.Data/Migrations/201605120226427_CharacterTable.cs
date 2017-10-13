namespace FeMMORPG.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CharacterTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Characters",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 64),
                        Surname = c.String(maxLength: 64),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.Name, unique: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Characters", "UserId", "dbo.Users");
            DropIndex("dbo.Characters", new[] { "UserId" });
            DropIndex("dbo.Characters", new[] { "Name" });
            DropTable("dbo.Characters");
        }
    }
}
