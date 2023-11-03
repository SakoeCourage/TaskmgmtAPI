using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Reflection.Metadata;
using TaskmgmtAPI.Models;
using UserTask = TaskmgmtAPI.Models.UserTask;

namespace TaskmgmtAPI.Db
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options) { }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany<UserTask>(e => e.UserTasks)
                .WithOne(e => e.author)
                .HasForeignKey(e => e.authorID)
                .IsRequired();

            modelBuilder.Entity<User>()
            .Property(b => b.createdAt)
            .HasDefaultValueSql("now()");


            modelBuilder.Entity<User>()
           .Property(b => b.updatedAt)
           .HasDefaultValueSql("now()");

            modelBuilder.Entity<UserTask>()
           .Property(b => b.createdAt)
           .HasDefaultValueSql("now()");

           modelBuilder.Entity<UserTask>()
          .Property(b => b.updatedAt)
          .HasDefaultValueSql("now()");



            //modelBuilder.Entity<User>().HasData(
            //      new User
            //      {   id = 1,
            //          name = "Sakoe courage",
            //          email = "akorlicourage@gmail.com",
            //          password = BCrypt.Net.BCrypt.HashPassword("itsapassword"),
            //          createdAt = DateTime.Parse("2023-10-25 13:47:00"),
            //          updatedAt = DateTime.Parse("2023-10-25 13:47:00"),
            //      },
            //     new User
            //     {
            //         id= 2,
            //         name = "akorlicourage",
            //         email = "ceejay995@live.com",
            //         password = BCrypt.Net.BCrypt.HashPassword("itsapassword123"),
            //         createdAt = DateTime.Parse("2023-10-25 13:47:00"),
            //         updatedAt = DateTime.Parse("2023-10-25 13:47:00"),
            //     }
            //    );
        }

        public DbSet<UserTask> UserTask { get; set; }

        public DbSet<User> User { get; set; }

    }
}
