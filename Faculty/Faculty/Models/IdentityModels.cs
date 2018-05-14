using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Faculty.Models.subjects;
using Faculty.Models.users;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Faculty.Models
{
    
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Worker> Workers { get; set; }
        public DbSet<Discipline> Disciplines { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Seminar> Seminars { get; set; }
        public DbSet<Lecture> Lectures { get; set; }
        public DbSet<SystemAction> SystemActions { get; set; }
        public DbSet<RegistrationRequest> RegistrationRequests { get; set; }
        public DbSet<CourseSubscriptionRequest> CourseSubscriptionRequests { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<StudentCource> StudentCources { get; set; }
        public DbSet<Group> Groups { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}