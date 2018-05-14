namespace Faculty.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RegistrationRequestMigration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RegistrationRequests", "GroupId", c => c.Int(nullable: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RegistrationRequests", "GroupId");
        }
    }
}
