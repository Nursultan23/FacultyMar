namespace Faculty.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ContextMigration : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Notifications", "Title", c => c.String(nullable: false));
            AlterColumn("dbo.Notifications", "Body", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Notifications", "Body", c => c.String());
            AlterColumn("dbo.Notifications", "Title", c => c.String());
        }
    }
}
