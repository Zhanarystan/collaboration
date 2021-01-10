using System.Collections.Generic;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Collaboration.Models
{
    // В профиль пользователя можно добавить дополнительные данные, если указать больше свойств для класса ApplicationUser. Подробности см. на странице https://go.microsoft.com/fwlink/?LinkID=317594.
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public int? SpecificationId { get; set; }
        public virtual Specifications? Specification { get; set; }

        public byte[] Image { get; set; }
        public virtual ICollection<Projects> Projects { get; set; }
        public virtual ICollection<Enrollments> Enrollments { get; set; }
        public virtual ICollection<Works> Works { get; set; }
        public virtual ICollection<EnrollmentRequests> EnrollmentRequests { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Обратите внимание, что authenticationType должен совпадать с типом, определенным в CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Здесь добавьте утверждения пользователя
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("ApplicationDbContext", throwIfV1Schema: false)
        {
        }

        public DbSet<Projects> Projects { get; set; }
        public DbSet<Specifications> Specifications { get; set; }
        public DbSet<Enrollments> Enrollments { get; set; }
        public DbSet<Works> Works { get; set; }
        public DbSet<EnrollmentRequests> EnrollmentRequests { get; set; }
        public DbSet<Countries> Countries { get; set; }

        public DbSet<CountriesViewModel> CountriesViewModel { get; set; }
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationUser>().HasOptional(c => c.Specification).
                WithMany(x => x.Users).HasForeignKey(x => x.SpecificationId);

            modelBuilder.Entity<ApplicationUser>().HasMany(c => c.Projects).
                WithOptional(x => x.PostedBy).HasForeignKey(x => x.PostedById);

            modelBuilder.Entity<Enrollments>().HasOptional(c => c.User).
                WithMany(x => x.Enrollments).HasForeignKey(x => x.UserId);

            modelBuilder.Entity<Enrollments>().HasOptional(c => c.Project).
                WithMany(x => x.Enrollments).HasForeignKey(x => x.ProjectId);

            modelBuilder.Entity<Projects>().HasMany(c => c.Countries)
            .WithMany(s => s.Projects)
            .Map(t => t.MapLeftKey("ProjectId")
            .MapRightKey("CountryId")
            .ToTable("CountryProject"));

            modelBuilder.Entity<EnrollmentRequests>().HasOptional(c => c.User).
                WithMany(x => x.EnrollmentRequests).HasForeignKey(x => x.UserId);

            modelBuilder.Entity<EnrollmentRequests>().HasOptional(c => c.Project).
                WithMany(x => x.EnrollmentRequests).HasForeignKey(x => x.ProjectId);

            modelBuilder.Entity<ApplicationUser>().HasMany(c => c.Works).
                WithOptional(x => x.User).HasForeignKey(x => x.UserId);

            base.OnModelCreating(modelBuilder);

        }
    }



}