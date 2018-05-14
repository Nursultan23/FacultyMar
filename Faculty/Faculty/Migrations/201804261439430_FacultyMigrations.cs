namespace Faculty.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FacultyMigrations : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Courses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        DisciplineId = c.Int(nullable: true),
                        isActive = c.Boolean(nullable: true),
                        Stage = c.Byte(nullable: true),
                        IsElective = c.Boolean(nullable: true),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CourseSubscriptionRequests",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StudentId = c.Int(nullable: true),
                        CourseId = c.Int(nullable: true),
                        status = c.Int(nullable: true),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Disciplines",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: true),
                        Description = c.String(nullable: true),
                        Literatures = c.String(),
                        IsActive = c.Boolean(nullable: true),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Groups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Stage = c.Byte(nullable: true),
                        isActive = c.Boolean(nullable: true),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Lectures",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CourseId = c.Int(nullable: true),
                        CreditCount = c.Byte(nullable: true),
                        ProcessorId = c.Int(nullable: true),
                        Time = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Notifications",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Body = c.String(),
                        Enable = c.Boolean(nullable: true),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RegistrationRequests",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Firstname = c.String(nullable: true),
                        MiddleName = c.String(nullable: true),
                        LastName = c.String(nullable: true),
                        Email = c.String(nullable: true),
                        TelephoneNumber = c.String(nullable: true),
                        Password = c.String(),
                        status = c.Int(nullable: true),
                        AdditionalTime = c.DateTime(nullable: true),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Seminars",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CourseId = c.Int(nullable: true),
                        CreditCount = c.Byte(nullable: true),
                        ProcessorId = c.Int(nullable: true),
                        Time = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.StudentCources",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StudentId = c.Int(nullable: true),
                        CourceId = c.Int(nullable: true),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Students",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Firstname = c.String(),
                        MiddleName = c.String(),
                        LastName = c.String(),
                        userId = c.String(),
                        UserName = c.String(),
                        AddmissionTime = c.DateTime(nullable: true),
                        GraduationTime = c.DateTime(nullable: true),
                        Status = c.Int(nullable: true),
                        Stage = c.Byte(nullable: true),
                        GroupId = c.Int(nullable: true),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SystemActions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Time = c.DateTime(nullable: true),
                        Description = c.String(),
                        ActorId = c.Int(nullable: true),
                        SubjectId = c.Int(nullable: true),
                        Type = c.Int(nullable: true),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Workers",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        Firstname = c.String(),
                        MiddleName = c.String(),
                        LastName = c.String(),
                        UserId = c.String(),
                        UserName = c.String(),
                        ScienceDegree = c.String(),
                        Status = c.Int(nullable: true),
                        isItProfessor = c.Boolean(nullable: true),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Workers");
            DropTable("dbo.SystemActions");
            DropTable("dbo.Students");
            DropTable("dbo.StudentCources");
            DropTable("dbo.Seminars");
            DropTable("dbo.RegistrationRequests");
            DropTable("dbo.Notifications");
            DropTable("dbo.Lectures");
            DropTable("dbo.Groups");
            DropTable("dbo.Disciplines");
            DropTable("dbo.CourseSubscriptionRequests");
            DropTable("dbo.Courses");
        }
    }
}
