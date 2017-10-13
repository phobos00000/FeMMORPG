namespace FeMMORPG.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class MoreUserFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "LastLogout", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AddColumn("dbo.Users", "SecondsPlayed", c => c.Int(nullable: false, defaultValue: 0));
        }

        public override void Down()
        {
            DropColumn("dbo.Users", "SecondsPlayed");
            DropColumn("dbo.Users", "LastLogout");
        }
    }
}
