using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using STimesheet.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace STimesheet.DBcontext
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        /* protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
         {
             var builder = new ConfigurationBuilder()
             .SetBasePath
             (Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.json");



             var configuration = builder.Build();
             optionsBuilder.UseSqlServer(configuration.GetConnectionString("SQL"));
            *//* optionsBuilder.Entity<TUserRole>().HasKey(r => new { r.UserId, r.RoleId })
             .ToTable("AspNetUserRoles");*//*
             //optionsBuilder.UseSqlServer(configuration.GetConnectionString("SQLRemoteConnection")); 
         }*/

        public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUserRoles> AspNetUserRoles { get; set; }
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
        public virtual DbSet<Clients> Clients { get; set; }
        public virtual DbSet<Department> Department { get; set; }
        public virtual DbSet<Designation> Designation { get; set; }
        public virtual DbSet<Employee> Employee { get; set; }
        public virtual DbSet<EmployeeProjects> EmployeeProjects { get; set; }
        public virtual DbSet<MigrationHistory> MigrationHistory { get; set; }
        public virtual DbSet<PasswordReset> PasswordReset { get; set; }
        public virtual DbSet<PowerBiLinks> PowerBiLinks { get; set; }
        public virtual DbSet<Project> Project { get; set; }
        public virtual DbSet<ProjectType> ProjectType { get; set; }
        public virtual DbSet<Tasks> Tasks { get; set; }
        public virtual DbSet<TimeSheetLogTable> TimeSheetLogTable { get; set; }
        public virtual DbSet<TimeSheetLogTable2> TimeSheetLogTable2 { get; set; }
        public virtual DbSet<TimesheetDetails> TimesheetDetails { get; set; }
        public virtual DbSet<UserCredentials> UserCredentials { get; set; }
        public virtual DbSet<UserType> UserType { get; set; }


         
   }
}
