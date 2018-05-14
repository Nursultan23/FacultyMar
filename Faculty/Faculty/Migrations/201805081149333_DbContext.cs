namespace Faculty.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DbContext : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Groups", "Name", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Groups", "Name", c => c.String());
        }
    }
}
