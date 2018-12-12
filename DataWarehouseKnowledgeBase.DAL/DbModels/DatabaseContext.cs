using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DataWarehouseKnowledgeBase.DAL.DbModels
{
    public partial class DatabaseContext : DbContext
    {
        public DatabaseContext()
        {
        }

        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Brand> Brand { get; set; }
        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<Country> Country { get; set; }
        public virtual DbSet<Locations> Locations { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<Sales> Sales { get; set; }
        public virtual DbSet<Store> Store { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
    
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Brand>(entity =>
            {
                entity.ToTable("brand");

                entity.Property(e => e.BrandId).HasColumnName("brand_id");

                entity.Property(e => e.BrandName)
                    .IsRequired()
                    .HasColumnName("brand_name")
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("category");

                entity.Property(e => e.CategoryId).HasColumnName("category_id");

                entity.Property(e => e.CategoryName)
                    .IsRequired()
                    .HasColumnName("category_name")
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Country>(entity =>
            {
                entity.ToTable("country");

                entity.Property(e => e.CountryId).HasColumnName("country_id");

                entity.Property(e => e.CountryName)
                    .IsRequired()
                    .HasColumnName("country_name")
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Locations>(entity =>
            {
                entity.HasKey(e => e.LocationId);

                entity.ToTable("locations");

                entity.Property(e => e.LocationId).HasColumnName("location_id");

                entity.Property(e => e.CityName)
                    .IsRequired()
                    .HasColumnName("city_name")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.CountryCountryId).HasColumnName("country_country_id");

                entity.HasOne(d => d.CountryCountry)
                    .WithMany(p => p.Locations)
                    .HasForeignKey(d => d.CountryCountryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("location_country_fk");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("product");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.BrandBrandId).HasColumnName("brand_brand_id");

                entity.Property(e => e.CategoryCategoryId).HasColumnName("category_category_id");

                entity.Property(e => e.InWarehouse).HasColumnName("in_warehouse");

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

                entity.HasOne(d => d.BrandBrand)
                    .WithMany(p => p.Product)
                    .HasForeignKey(d => d.BrandBrandId)
                    .HasConstraintName("product_brand_fk");

                entity.HasOne(d => d.CategoryCategory)
                    .WithMany(p => p.Product)
                    .HasForeignKey(d => d.CategoryCategoryId)
                    .HasConstraintName("product_category_fk");
            });

            modelBuilder.Entity<Sales>(entity =>
            {
                entity.HasKey(e => e.SaleId);

                entity.ToTable("sales");

                entity.Property(e => e.SaleId).HasColumnName("sale_id");

                entity.Property(e => e.InWarehouse).HasColumnName("in_warehouse");

                entity.Property(e => e.Price)
                    .HasColumnName("price")
                    .HasColumnType("decimal(10, 2)");

                entity.Property(e => e.ProductProductId).HasColumnName("product_product_id");

                entity.Property(e => e.SaleDate)
                    .HasColumnName("sale_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.StoreStoreId).HasColumnName("store_store_id");

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
            });

            modelBuilder.Entity<Store>(entity =>
            {
                entity.ToTable("store");

                entity.Property(e => e.StoreId).HasColumnName("store_id");

                entity.Property(e => e.InWarehouse).HasColumnName("in_warehouse");

                entity.Property(e => e.LocationLocationId).HasColumnName("location_location_id");

                entity.Property(e => e.StoreNumber)
                    .IsRequired()
                    .HasColumnName("store_number")
                    .HasMaxLength(13)
                    .IsUnicode(false);

                entity.HasOne(d => d.LocationLocation)
                    .WithMany(p => p.Store)
                    .HasForeignKey(d => d.LocationLocationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("store_location_fk");
            });
        }
    }
}
