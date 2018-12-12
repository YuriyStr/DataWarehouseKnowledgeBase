using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DataWarehouseKnowledgeBase.DAL.DwModels
{
    public partial class DwContext : DbContext
    {
        public DwContext()
        {
        }

        public DwContext(DbContextOptions<DwContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<Sales> Sales { get; set; }
        public virtual DbSet<Store> Store { get; set; }
        public virtual DbSet<Times> Times { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                //optionsBuilder.UseSqlServer();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("product");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.BrandName)
                    .HasColumnName("brand_name")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ProductCategory)
                    .HasColumnName("product_category")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ProductCode)
                    .IsRequired()
                    .HasColumnName("product_code")
                    .HasMaxLength(13)
                    .IsUnicode(false);

                entity.Property(e => e.ProductName)
                    .IsRequired()
                    .HasColumnName("product_name")
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Sales>(entity =>
            {
                entity.HasKey(e => new { e.ProductProductId, e.StoreStoreId, e.TimeTimeId });

                entity.ToTable("sales");

                entity.Property(e => e.ProductProductId).HasColumnName("product_product_id");

                entity.Property(e => e.StoreStoreId).HasColumnName("store_store_id");

                entity.Property(e => e.TimeTimeId).HasColumnName("time_time_id");

                entity.Property(e => e.MoneySold)
                    .HasColumnName("money_sold")
                    .HasColumnType("decimal(10, 2)");

                entity.Property(e => e.UnitsSold)
                    .HasColumnName("units_sold")
                    .HasColumnType("decimal(10, 2)");

                entity.HasOne(d => d.ProductProduct)
                    .WithMany(p => p.Sales)
                    .HasForeignKey(d => d.ProductProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("sales_product_fk");

                entity.HasOne(d => d.StoreStore)
                    .WithMany(p => p.Sales)
                    .HasForeignKey(d => d.StoreStoreId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("sales_store_fk");

                entity.HasOne(d => d.TimeTime)
                    .WithMany(p => p.Sales)
                    .HasForeignKey(d => d.TimeTimeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("sales_time_fk");
            });

            modelBuilder.Entity<Store>(entity =>
            {
                entity.ToTable("store");

                entity.Property(e => e.StoreId).HasColumnName("store_id");

                entity.Property(e => e.City)
                    .IsRequired()
                    .HasColumnName("city")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Country)
                    .IsRequired()
                    .HasColumnName("country")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.StoreNumber)
                    .IsRequired()
                    .HasColumnName("store_number")
                    .HasMaxLength(13)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Times>(entity =>
            {
                entity.HasKey(e => e.TimeId);

                entity.ToTable("times");

                entity.Property(e => e.TimeId).HasColumnName("time_id");

                entity.Property(e => e.Day).HasColumnName("day");

                entity.Property(e => e.MonthName)
                    .HasColumnName("month_name")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.MonthNo).HasColumnName("month_no");

                entity.Property(e => e.WeekDay)
                    .HasColumnName("week_day")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Year).HasColumnName("year");
            });
        }
    }
}
