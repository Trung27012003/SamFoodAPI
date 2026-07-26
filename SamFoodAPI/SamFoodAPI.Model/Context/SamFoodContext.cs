using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SamFoodAPI.Model.Entities;

namespace SamFoodAPI.Model.Context;

public partial class SamFoodContext : DbContext
{
    public SamFoodContext(DbContextOptions<SamFoodContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductIngredient> ProductIngredients { get; set; }

    public virtual DbSet<ProductProcessingRecipe> ProductProcessingRecipes { get; set; }

    public virtual DbSet<UnitCount> UnitCounts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Category");

            entity.Property(e => e.CategoryCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CategoryName).HasMaxLength(550);
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.ParentID).HasDefaultValue(0);
            entity.Property(e => e.STT).HasDefaultValue(0);
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Product");

            entity.Property(e => e.CategoryID).HasDefaultValue(0);
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.Origin).HasMaxLength(650);
            entity.Property(e => e.ProductCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ProductName).HasMaxLength(550);
            entity.Property(e => e.STT).HasDefaultValue(0);
            entity.Property(e => e.Status).HasDefaultValue(0);
            entity.Property(e => e.UnitCountID).HasDefaultValue(0);
            entity.Property(e => e.UnitPrice)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.Weight)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<ProductIngredient>(entity =>
        {
            entity.ToTable("ProductIngredient");

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IngredientName).HasMaxLength(550);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.ProductID).HasDefaultValue(0);
            entity.Property(e => e.Quantity)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UnitCountID).HasDefaultValue(0);
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ProductProcessingRecipe>(entity =>
        {
            entity.ToTable("ProductProcessingRecipe");

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.ProductID).HasDefaultValue(0);
            entity.Property(e => e.Step).HasDefaultValue(0);
            entity.Property(e => e.StepName).HasMaxLength(250);
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<UnitCount>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK_Unit");

            entity.ToTable("UnitCount");

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.UnitName).HasMaxLength(250);
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
