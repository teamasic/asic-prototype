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

        public MainDbContext(DbContextOptions<MainDbContext> options): base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Attendee>(entity =>
            {
                entity.HasKey(e => e.Code);

                entity.Property(e => e.Code).HasColumnType("varchar ( 50 )");

                entity.Property(e => e.Name).HasColumnType("varchar ( 255 )");
            });

            modelBuilder.Entity<AttendeeGroup>(entity =>
            {
                entity.HasIndex(e => new { e.AttendeeCode, e.GroupCode })
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.AttendeeCode)
                    .IsRequired()
                    .HasColumnType("varchar ( 50 )");

                entity.Property(e => e.GroupCode)
                    .IsRequired()
                    .HasColumnType("varchar ( 50 )");

                entity.Property(e => e.IsActive).HasDefaultValueSql("1");

                entity.HasOne(d => d.Attendee)
                    .WithMany(p => p.AttendeeGroup)
                    .HasForeignKey(d => d.AttendeeCode)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.AttendeeGroups)
                    .HasForeignKey(d => d.GroupCode)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<ChangeRequest>(entity =>
            {
                entity.HasIndex(e => e.RecordId)
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Comment).HasColumnType("varchar ( 255 )");

                entity.Property(e => e.RecordId).HasColumnType("int");

                entity.Property(e => e.Status).HasColumnType("int");

                entity.HasOne(d => d.Record)
                    .WithOne(p => p.ChangeRequest)
                    .HasForeignKey<ChangeRequest>(d => d.RecordId);
            });

            modelBuilder.Entity<Group>(entity =>
            {
                entity.HasKey(e => e.Code);

                entity.Property(e => e.Code).HasColumnType("varchar ( 50 )");

                entity.Property(e => e.DateTimeCreated).HasColumnType("varchar ( 255 )");

                entity.Property(e => e.Deleted).HasDefaultValueSql("0");

                entity.Property(e => e.Name).HasColumnType("varchar ( 100 )");
            });

            modelBuilder.Entity<Record>(entity =>
            {
                entity.HasIndex(e => new { e.AttendeeGroupId, e.SessionId })
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.AttendeeCode)
                    .IsRequired()
                    .HasColumnType("varchar ( 50 )");

                entity.Property(e => e.EndTime)
                    .IsRequired()
                    .HasColumnType("varchar ( 255 )");

                entity.Property(e => e.SessionName)
                    .IsRequired()
                    .HasColumnType("varchar ( 255 )");

                entity.Property(e => e.StartTime)
                    .IsRequired()
                    .HasColumnType("varchar ( 255 )");

                entity.Property(e => e.UpdateTime).HasColumnType("varchar ( 255 )");

                entity.HasOne(d => d.AttendeeGroup)
                    .WithMany(p => p.Records)
                    .HasForeignKey(d => d.AttendeeGroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Session)
                    .WithMany(p => p.Records)
                    .HasForeignKey(d => d.SessionId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Room>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CameraConnectionString)
                    .IsRequired()
                    .HasColumnType("varchar ( 255 )");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar ( 50 )");
            });

            modelBuilder.Entity<Session>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.EndTime)
                    .IsRequired()
                    .HasColumnType("varchar ( 255 )");

                entity.Property(e => e.GroupCode)
                    .IsRequired()
                    .HasColumnType("varchar ( 50 )");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("nvarchar ( 50 )");

                entity.Property(e => e.RoomId).HasColumnType("int");

                entity.Property(e => e.StartTime)
                    .IsRequired()
                    .HasColumnType("varchar ( 255 )");

                entity.Property(e => e.Status).HasColumnType("varchar ( 50 )");

                entity.HasOne(d => d.Groups)
                    .WithMany(p => p.Sessions)
                    .HasForeignKey(d => d.GroupCode)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.Session)
                    .HasForeignKey(d => d.RoomId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });
        }
    }
}
