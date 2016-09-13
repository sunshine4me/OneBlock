using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace blockPlayDataEntities
{
    public partial class blockPlayDBContext : DbContext
    {
        public blockPlayDBContext(DbContextOptions<blockPlayDBContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PlayUser>(entity =>
            {
                entity.HasIndex(e => e.Username)
                    .HasName("sqlite_autoindex_PlayUser_1")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnType("integer");

                entity.Property(e => e.Avatar).HasColumnType("nvarchar(50)");

                entity.Property(e => e.JoinDate)
                    .IsRequired()
                    .HasColumnType("datetime");

                entity.Property(e => e.Locked).HasColumnType("bit");

                entity.Property(e => e.Lv).HasColumnType("smallint(1)");

                entity.Property(e => e.Name).HasColumnType("nvarchar(10)");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasColumnType("nvarchar(20)");
            });

            modelBuilder.Entity<TestCase>(entity =>
            {
                entity.Property(e => e.Id).HasColumnType("integer");

                entity.Property(e => e.Body).HasColumnType("ntext");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("nvarchar(50)");

                entity.Property(e => e.ParentId).HasColumnType("integer");
                

                entity.Property(e => e.Type).HasColumnType("tinyint");

                entity.Property(e => e.UserId)
                    .HasColumnName("UserID")
                    .HasColumnType("integer");

                entity.HasOne(d => d.Parent)
                    .WithMany(p => p.InverseParent)
                    .HasForeignKey(d => d.ParentId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.TestCase)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<TestSpace>(entity =>
            {
                entity.Property(e => e.Id).HasColumnType("integer");

                entity.Property(e => e.Describe).HasColumnType("nvarchar(100)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("nvarchar(20)");

                entity.Property(e => e.SapceData)
                    .IsRequired()
                    .HasColumnType("ntext");

                entity.Property(e => e.UserId).HasColumnType("int");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.TestSpace)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            

            modelBuilder.Entity<BlockStep>(entity =>
            {
                entity.Property(e => e.Id).HasColumnType("integer");

                entity.Property(e => e.Body)
                    .IsRequired()
                    .HasColumnType("ntext");
                entity.Property(e => e.Attrs)
                    .IsRequired()
                    .HasColumnType("ntext");


                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("nvarchar(50)");
                

                entity.Property(e => e.UserId).HasColumnType("integer");

              
                entity.HasOne(d => d.User)
                    .WithMany(p => p.BlockStep)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        public virtual DbSet<PlayUser> PlayUser { get; set; }
        public virtual DbSet<TestCase> TestCase { get; set; }
        public virtual DbSet<TestSpace> TestSpace { get; set; }
        public virtual DbSet<BlockStep> BlockStep { get; set; }
    }
}