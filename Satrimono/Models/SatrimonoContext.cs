using System;
using HanumanInstitute.CommonWeb;
using Microsoft.EntityFrameworkCore;

namespace HanumanInstitute.Satrimono.Models
{
    public partial class SatrimonoContext : DbContext
    {
        public SatrimonoContext()
        {
        }

        public SatrimonoContext(DbContextOptions<SatrimonoContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Book>? Book { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.CheckNotNull(nameof(modelBuilder));

            modelBuilder.HasAnnotation("ProductVersion", "2.2.2-servicing-10034");

            modelBuilder.Entity<Book>(entity =>
            {
                entity.HasIndex(e => e.Author).HasName("Index_Author");
                entity.HasIndex(e => e.Id).HasName("Index_BookId");
                entity.HasIndex(e => e.Key).HasName("Index_BookKey");

                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.Key).HasColumnType("TEXT COLLATE NOCASE").HasDefaultValue("");
                entity.Property(e => e.Title).HasColumnType("TEXT COLLATE NOCASE");
                entity.Property(e => e.Subtitle).HasColumnType("TEXT COLLATE NOCASE");
                entity.Property(e => e.Author).HasColumnType("TEXT COLLATE NOCASE");
                entity.Property(e => e.IsFiction).HasDefaultValue(false);
            });
        }
    }
}
