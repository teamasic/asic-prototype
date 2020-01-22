using AttendanceSystemIPCamera.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Framework.Database
{
    public class MainDbContext: DbContext
    {
        public DbSet<Attendee> Attendees { get; set; }
        public DbSet<AttendeeGroup> AttendeeGroups { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Record> Records { get; set; }
        public DbSet<Session> Sessions { get; set; }

        public MainDbContext(DbContextOptions<MainDbContext> options): base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<AttendeeGroup>()
                .HasKey(t => new { t.AttendeeId, t.GroupId });
        }
    }
}
