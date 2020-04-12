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
        public DbSet<Attendee> Attendee { get; set; }
        public DbSet<AttendeeGroup> AttendeeGroup { get; set; }
        public DbSet<Group> Group { get; set; }
        public DbSet<Record> Record { get; set; }
        public DbSet<Session> Session { get; set; }
        public DbSet<Room> Room { get; set; }
        public DbSet<ChangeRequest> ChangeRequest { get; set; }
        public DbSet<Schedule> Schedule { get; set; }

        public MainDbContext(DbContextOptions<MainDbContext> options): base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Attendee>(entity =>
            {
                entity.HasIndex(e => e.Code)
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Code).HasColumnType("varchar (255)");

                entity.Property(e => e.Name).HasColumnType("varchar (255)");
            });

            modelBuilder.Entity<AttendeeGroup>(entity =>
            {
                entity.HasKey(e => new { e.AttendeeId, e.GroupId });

                entity.HasIndex(e => e.GroupId)
                    .HasName("IX_AttendeeGroups_GroupId");

                entity.Property(e => e.IsActive).HasDefaultValueSql("1");

                entity.HasOne(d => d.Attendee)
                    .WithMany(p => p.AttendeeGroups)
                    .HasForeignKey(d => d.AttendeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.AttendeeGroups)
                    .HasForeignKey(d => d.GroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<ChangeRequest>(entity =>
            {
                entity.HasIndex(e => e.RecordId)
                    .HasName("IX_ChangeRequests_RecordId");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.RecordId).HasColumnType("int");

                entity.Property(e => e.Status).HasColumnType("int");

                entity.HasOne(d => d.Record)
                    .WithMany(p => p.ChangeRequest)
                    .HasForeignKey(d => d.RecordId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Group>(entity =>
            {
                entity.HasIndex(e => e.Code)
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Code).HasColumnType("varchar (255)");

                entity.Property(e => e.DateTimeCreated)
                    .IsRequired()
                    .HasColumnType("varchar (255)");

                entity.Property(e => e.Deleted).HasDefaultValueSql("0");

                entity.Property(e => e.Name).HasColumnType("varchar (255)");
            });

            modelBuilder.Entity<Record>(entity =>
            {
                entity.HasIndex(e => new { e.AttendeeId, e.GroupId, e.SessionId })
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.AttendeeCode)
                    .IsRequired()
                    .HasColumnType("varchar (255)");

                entity.Property(e => e.EndTime).IsRequired();

                entity.Property(e => e.SessionName)
                    .IsRequired()
                    .HasColumnType("varchar (255)");

                entity.Property(e => e.StartTime).IsRequired();

                entity.HasOne(d => d.Session)
                    .WithMany(p => p.Records)
                    .HasForeignKey(d => d.SessionId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.AttendeeGroup)
                    .WithMany(p => p.Record)
                    .HasForeignKey(d => new { d.AttendeeId, d.GroupId })
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Room>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar (255)");

                entity.Property(e => e.RtspString).IsRequired();
            });

            modelBuilder.Entity<Schedule>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Active).HasDefaultValueSql("0");

                entity.Property(e => e.EndTime)
                    .IsRequired()
                    .HasColumnType("varchar (255)");

                entity.Property(e => e.GroupId).HasColumnType("int");

                entity.Property(e => e.Room).IsRequired();

                entity.Property(e => e.Slot).IsRequired();

                entity.Property(e => e.StartTime)
                    .IsRequired()
                    .HasColumnType("varchar (255)");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.Schedules)
                    .HasForeignKey(d => d.GroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Session>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.EndTime)
                    .IsRequired()
                    .HasColumnType("varchar (255)");

                entity.Property(e => e.GroupId).HasColumnType("int");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar (255)");

                entity.Property(e => e.RoomName)
                    .IsRequired()
                    .HasColumnType("varchar (255)");

                entity.Property(e => e.RtspString).IsRequired();

                entity.Property(e => e.StartTime)
                    .IsRequired()
                    .HasColumnType("varchar (255)");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.Sessions)
                    .HasForeignKey(d => d.GroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });
        }
    }
}
