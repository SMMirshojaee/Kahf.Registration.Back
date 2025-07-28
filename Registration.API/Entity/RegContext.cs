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

    public virtual DbSet<Applicant> Applicants { get; set; }

    public virtual DbSet<ApplicantFormValue> ApplicantFormValues { get; set; }

    public virtual DbSet<Field> Fields { get; set; }

    public virtual DbSet<FieldOption> FieldOptions { get; set; }

    public virtual DbSet<FieldType> FieldTypes { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Models.Payment> Payments { get; set; }

    public virtual DbSet<Reg> Regs { get; set; }

    public virtual DbSet<RegCost> RegCosts { get; set; }

    public virtual DbSet<RegStep> RegSteps { get; set; }

    public virtual DbSet<RegStepStatus> RegStepStatuses { get; set; }

    public virtual DbSet<Step> Steps { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Applicant>(entity =>
        {
            entity.ToTable("Applicants", "applicant");

            entity.HasIndex(e => new { e.NationalNumber, e.RegId }, "IX_Applicants_1").IsUnique();

            entity.Property(e => e.CreatedDate)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Description).HasMaxLength(4000);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.NationalNumber)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(11)
                .IsFixedLength();
            entity.Property(e => e.TrackingCode)
                .HasMaxLength(5)
                .IsFixedLength();

            entity.HasOne(d => d.Leader).WithMany(p => p.InverseLeader)
                .HasForeignKey(d => d.LeaderId)
                .HasConstraintName("FK_Applicants_Applicants1");

            entity.HasOne(d => d.Reg).WithMany(p => p.Applicants)
                .HasForeignKey(d => d.RegId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Applicants_Regs");

            entity.HasOne(d => d.Status).WithMany(p => p.Applicants)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK_Applicants_RegStepStatuses");
        });

        modelBuilder.Entity<ApplicantFormValue>(entity =>
        {
            entity.ToTable("ApplicantFormValues", "applicant");

            entity.Property(e => e.CreatedDate)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Applicant).WithMany(p => p.ApplicantFormValues)
                .HasForeignKey(d => d.ApplicantId)
                .HasConstraintName("FK_ApplicantFormValues_Applicants");

            entity.HasOne(d => d.Field).WithMany(p => p.ApplicantFormValues)
                .HasForeignKey(d => d.FieldId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ApplicantFormValues_Fields");

            entity.HasOne(d => d.FieldOption).WithMany(p => p.ApplicantFormValues)
                .HasForeignKey(d => d.FieldOptionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ApplicantFormValues_FieldOptions");

            entity.HasOne(d => d.RegStep).WithMany(p => p.ApplicantFormValues)
                .HasForeignKey(d => d.RegStepId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ApplicantFormValues_RegSteps");
        });

        modelBuilder.Entity<Field>(entity =>
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

        modelBuilder.Entity<FieldOption>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_FormOptions");

            entity.ToTable("FieldOptions", "field");

            entity.Property(e => e.CreatedDate)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Title).HasMaxLength(100);
            entity.Property(e => e.Type).HasMaxLength(50);
            entity.Property(e => e.Value).HasMaxLength(4000);

            entity.HasOne(d => d.Field).WithMany(p => p.FieldOptions)
                .HasForeignKey(d => d.FieldId)
                .HasConstraintName("FK_FieldOptions_Fields");
        });

        modelBuilder.Entity<FieldType>(entity =>
        {
            entity.ToTable("FieldTypes", "field");

            entity.Property(e => e.CreatedDate)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(50);
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.ToTable("Messages", "applicant");

            entity.Property(e => e.CreatedDate)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Mobile).HasMaxLength(20);
            entity.Property(e => e.NationalNumber)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.Text).HasMaxLength(4000);

            entity.HasOne(d => d.Applicant).WithMany(p => p.Messages)
                .HasForeignKey(d => d.ApplicantId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Messages_Applicants");

            entity.HasOne(d => d.User).WithMany(p => p.Messages)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Messages_Users");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("Orders", "applicant");

            entity.Property(e => e.Authority).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.NationalNumber)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.RequestContent).HasMaxLength(4000);
            entity.Property(e => e.RequestDate).HasPrecision(0);
            entity.Property(e => e.VerifyContent).HasMaxLength(4000);
            entity.Property(e => e.VerifyDate).HasPrecision(0);

            entity.HasOne(d => d.Applicant).WithMany(p => p.Orders)
                .HasForeignKey(d => d.ApplicantId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Orders_Applicants");

            entity.HasOne(d => d.RegStep).WithMany(p => p.Orders)
                .HasForeignKey(d => d.RegStepId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Orders_RegSteps");
        });

        modelBuilder.Entity<Models.Payment>(entity =>
        {
            entity.ToTable("Payment", "pay");

            entity.Property(e => e.CreatedDate)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Description).HasMaxLength(1000);

            entity.HasOne(d => d.LoanStatus).WithMany(p => p.Payments)
                .HasForeignKey(d => d.LoanStatusId)
                .HasConstraintName("FK_Payment_RegStepStatuses");

            entity.HasOne(d => d.RegStep).WithMany(p => p.Payments)
                .HasForeignKey(d => d.RegStepId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Payment_RegSteps");
        });

        modelBuilder.Entity<Reg>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Registrations");

            entity.ToTable("Regs", "reg");

            entity.Property(e => e.CreatedDate)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.EndDate).HasPrecision(0);
            entity.Property(e => e.ImageAddress).HasMaxLength(50);
            entity.Property(e => e.StartDate).HasPrecision(0);
            entity.Property(e => e.Title).HasMaxLength(100);
        });

        modelBuilder.Entity<RegCost>(entity =>
        {
            entity.ToTable("RegCosts", "reg");

            entity.Property(e => e.CreatedDate)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Description).HasMaxLength(4000);
            entity.Property(e => e.Title).HasMaxLength(500);

            entity.HasOne(d => d.Reg).WithMany(p => p.RegCosts)
                .HasForeignKey(d => d.RegId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RegCosts_Regs");
        });

        modelBuilder.Entity<RegStep>(entity =>
        {
            entity.ToTable("RegSteps", "reg");

            entity.Property(e => e.AddMemberDescription).HasMaxLength(1000);
            entity.Property(e => e.CreatedDate)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Description).HasMaxLength(4000);
            entity.Property(e => e.MemberLimit).HasDefaultValue((byte)0);
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

        modelBuilder.Entity<RegStepStatus>(entity =>
        {
            entity.ToTable("RegStepStatuses", "reg");

            entity.Property(e => e.CreatedDate)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.PublicMessage).HasMaxLength(1000);
            entity.Property(e => e.Title).HasMaxLength(50);

            entity.HasOne(d => d.RegStep).WithMany(p => p.RegStepStatuses)
                .HasForeignKey(d => d.RegStepId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RegStepStatuses_RegSteps");
        });

        modelBuilder.Entity<Step>(entity =>
        {
            entity.ToTable("Steps", "reg");

            entity.Property(e => e.CreatedDate)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Title).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users", "admin");

            entity.Property(e => e.CreatedDate)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.HashedPassword).HasMaxLength(50);
            entity.Property(e => e.PasswordSalt).HasMaxLength(50);
            entity.Property(e => e.Role).HasMaxLength(50);
            entity.Property(e => e.Username).HasMaxLength(20);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
