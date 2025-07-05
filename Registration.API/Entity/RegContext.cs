using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Registration.API.Entity.Models;

namespace Registration.API.Entity;

public partial class RegContext : DbContext
{
    public RegContext(DbContextOptions<RegContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ApplicantFormValues> ApplicantFormValues { get; set; }

    public virtual DbSet<Applicants> Applicants { get; set; }

    public virtual DbSet<FieldOptions> FieldOptions { get; set; }

    public virtual DbSet<FieldTypes> FieldTypes { get; set; }

    public virtual DbSet<Fields> Fields { get; set; }

    public virtual DbSet<RegStepStatuses> RegStepStatuses { get; set; }

    public virtual DbSet<RegSteps> RegSteps { get; set; }

    public virtual DbSet<Regs> Regs { get; set; }

    public virtual DbSet<Steps> Steps { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApplicantFormValues>(entity =>
        {
            entity.ToTable("ApplicantFormValues", "applicant");

            entity.Property(e => e.CreatedDate)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Applicant).WithMany(p => p.ApplicantFormValues)
                .HasForeignKey(d => d.ApplicantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ApplicantFormValues_Applicants");

            entity.HasOne(d => d.Field).WithMany(p => p.ApplicantFormValues)
                .HasForeignKey(d => d.FieldId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ApplicantFormValues_Fields");

            entity.HasOne(d => d.FieldOption).WithMany(p => p.ApplicantFormValues)
                .HasForeignKey(d => d.FieldOptionId)
                .HasConstraintName("FK_ApplicantFormValues_FieldOptions");
        });

        modelBuilder.Entity<Applicants>(entity =>
        {
            entity.ToTable("Applicants", "applicant");

            entity.Property(e => e.CreatedDate)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.NationalNumber)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.TrackingCode)
                .HasMaxLength(5)
                .IsFixedLength();
        });

        modelBuilder.Entity<FieldOptions>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_FormOptions");

            entity.ToTable("FieldOptions", "field");

            entity.Property(e => e.CreatedDate)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Title).HasMaxLength(50);
            entity.Property(e => e.Type).HasMaxLength(50);
            entity.Property(e => e.Value).HasMaxLength(1000);

            entity.HasOne(d => d.Field).WithMany(p => p.FieldOptions)
                .HasForeignKey(d => d.FieldId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FieldOptions_Fields");
        });

        modelBuilder.Entity<FieldTypes>(entity =>
        {
            entity.ToTable("FieldTypes", "field");

            entity.Property(e => e.CreatedDate)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(50);
        });

        modelBuilder.Entity<Fields>(entity =>
        {
            entity.ToTable("Fields", "field");

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
            entity.ToTable("RegStepStatuses", "reg");

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
            entity.ToTable("RegSteps", "reg");

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

            entity.ToTable("Regs", "reg");

            entity.Property(e => e.CreatedDate)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.EndDate).HasPrecision(0);
            entity.Property(e => e.StartDate).HasPrecision(0);
            entity.Property(e => e.Title).HasMaxLength(100);
        });

        modelBuilder.Entity<Steps>(entity =>
        {
            entity.ToTable("Steps", "reg");

            entity.Property(e => e.CreatedDate)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Title).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
