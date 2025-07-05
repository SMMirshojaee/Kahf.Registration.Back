using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Registration.API.Entity.Models;

namespace Registration.API.Entity;

public partial class RegContext : DbContext
{
    public RegContext()
    {
    }

    public RegContext(DbContextOptions<RegContext> options)
        : base(options)
    {
    }

    public virtual DbSet<FieldTypes> FieldTypes { get; set; }

    public virtual DbSet<Fields> Fields { get; set; }

    public virtual DbSet<RegStepStatuses> RegStepStatuses { get; set; }

    public virtual DbSet<RegSteps> RegSteps { get; set; }

    public virtual DbSet<Regs> Regs { get; set; }

    public virtual DbSet<Steps> Steps { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.;Database=Kahf.Registration;User Id=sa;Password=master;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FieldTypes>(entity =>
        {
            entity.Property(e => e.CreatedDate)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(50);
        });

        modelBuilder.Entity<Fields>(entity =>
        {
            entity.Property(e => e.CreatedDate)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.FieldType).WithMany(p => p.Fields)
                .HasForeignKey(d => d.FieldTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Fields_FieldTypes");

            entity.HasOne(d => d.RegStep).WithMany(p => p.Fields)
                .HasForeignKey(d => d.RegStepId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Fields_RegSteps");
        });

        modelBuilder.Entity<RegStepStatuses>(entity =>
        {
            entity.Property(e => e.CreatedDate)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Title).HasMaxLength(50);

            entity.HasOne(d => d.RegStep).WithMany(p => p.RegStepStatuses)
                .HasForeignKey(d => d.RegStepId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RegStepStatuses_RegSteps");
        });

        modelBuilder.Entity<RegSteps>(entity =>
        {
            entity.Property(e => e.CreatedDate)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Title).HasMaxLength(100);

            entity.HasOne(d => d.Reg).WithMany(p => p.RegSteps)
                .HasForeignKey(d => d.RegId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RegSteps_Regs");

            entity.HasOne(d => d.Step).WithMany(p => p.RegSteps)
                .HasForeignKey(d => d.StepId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RegSteps_Steps");
        });

        modelBuilder.Entity<Regs>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Registrations");

            entity.Property(e => e.CreatedDate)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.EndDate).HasPrecision(0);
            entity.Property(e => e.StartDate).HasPrecision(0);
            entity.Property(e => e.Title).HasMaxLength(100);
        });

        modelBuilder.Entity<Steps>(entity =>
        {
            entity.Property(e => e.CreatedDate)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Title).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
