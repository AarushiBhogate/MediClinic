using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MediClinic.Models;

public partial class MediClinicDbContext : DbContext
{
    public MediClinicDbContext()
    {
    }

    public MediClinicDbContext(DbContextOptions<MediClinicDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<Chemist> Chemists { get; set; }

    public virtual DbSet<Drug> Drugs { get; set; }

    public virtual DbSet<DrugRequest> DrugRequests { get; set; }

    public virtual DbSet<Patient> Patients { get; set; }

    public virtual DbSet<Physician> Physicians { get; set; }

    public virtual DbSet<PhysicianAdvice> PhysicianAdvices { get; set; }

    public virtual DbSet<PhysicianPrescrip> PhysicianPrescrips { get; set; }

    public virtual DbSet<PurchaseOrderHeader> PurchaseOrderHeaders { get; set; }

    public virtual DbSet<PurchaseProductLine> PurchaseProductLines { get; set; }

    public virtual DbSet<Schedule> Schedules { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<User> Users { get; set; }
    


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=(localdb)\\mssqllocaldb;Initial Catalog=MediClinicDB;Integrated Security=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.AppointmentId).HasName("PK__Appointm__8ECDFCA22DB0FA67");

            entity.ToTable("Appointment");

            entity.Property(e => e.AppointmentId).HasColumnName("AppointmentID");
            entity.Property(e => e.AppointmentDate).HasColumnType("datetime");
            entity.Property(e => e.Criticality)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Note)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PatientId).HasColumnName("PatientID");
            entity.Property(e => e.Reason)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ScheduleStatus)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.Patient).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.PatientId)
                .HasConstraintName("FK_Appointment_Patient");
        });

        modelBuilder.Entity<Chemist>(entity =>
        {
            entity.HasKey(e => e.ChemistId).HasName("PK__Chemist__C0D5B7B4C293BF6D");

            entity.ToTable("Chemist");

            entity.Property(e => e.ChemistId).HasColumnName("ChemistID");
            entity.Property(e => e.Address)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.ChemistName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ChemistStatus)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Summary)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Drug>(entity =>
        {
            entity.HasKey(e => e.DrugId).HasName("PK__Drug__908D66F624B6FE4D");

            entity.ToTable("Drug");

            entity.Property(e => e.DrugId).HasColumnName("DrugID");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Dosage)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.DrugStatus)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.DrugTitle)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<DrugRequest>(entity =>
        {
            entity.HasKey(e => e.DrugRequestId).HasName("PK__DrugRequ__AEE9D65075240E95");

            entity.ToTable("DrugRequest");

            entity.Property(e => e.DrugRequestId).HasColumnName("DrugRequestID");
            entity.Property(e => e.DrugsInfoText).IsUnicode(false);
            entity.Property(e => e.PhysicianId).HasColumnName("PhysicianID");
            entity.Property(e => e.RequestDate).HasColumnType("datetime");
            entity.Property(e => e.RequestStatus)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.Physician).WithMany(p => p.DrugRequests)
                .HasForeignKey(d => d.PhysicianId)
                .HasConstraintName("FK_DrugRequest_Physician");
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.PatientId).HasName("PK__Patient__970EC34688E81DE8");

            entity.ToTable("Patient");

            entity.Property(e => e.PatientId).HasColumnName("PatientID");
            entity.Property(e => e.Address)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Dob).HasColumnName("DOB");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.PatientName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.PatientStatus)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Summary)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Physician>(entity =>
        {
            entity.HasKey(e => e.PhysicianId).HasName("PK__Physicia__DFF5ED73B26EE8C2");

            entity.ToTable("Physician");

            entity.Property(e => e.PhysicianId).HasColumnName("PhysicianID");
            entity.Property(e => e.Address)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.PhysicianName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.PhysicianStatus)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Specialization)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Summary)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<PhysicianAdvice>(entity =>
        {
            entity.HasKey(e => e.PhysicianAdviceId).HasName("PK__Physicia__82C62610A3F67EE2");

            entity.ToTable("PhysicianAdvice");

            entity.Property(e => e.PhysicianAdviceId).HasColumnName("PhysicianAdviceID");
            entity.Property(e => e.Advice)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Note)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ScheduleId).HasColumnName("ScheduleID");

            entity.HasOne(d => d.Schedule).WithMany(p => p.PhysicianAdvices)
                .HasForeignKey(d => d.ScheduleId)
                .HasConstraintName("FK_PhysicianAdvice_Schedule");
        });

        modelBuilder.Entity<PhysicianPrescrip>(entity =>
        {
            entity.HasKey(e => e.PrescriptionId).HasName("PK__Physicia__40130812C544648D");

            entity.ToTable("PhysicianPrescrip");

            entity.Property(e => e.PrescriptionId).HasColumnName("PrescriptionID");
            entity.Property(e => e.Dosage)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.DrugId).HasColumnName("DrugID");
            entity.Property(e => e.PhysicianAdviceId).HasColumnName("PhysicianAdviceID");
            entity.Property(e => e.Prescription)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Drug).WithMany(p => p.PhysicianPrescrips)
                .HasForeignKey(d => d.DrugId)
                .HasConstraintName("FK_Prescrip_Drug");

            entity.HasOne(d => d.PhysicianAdvice).WithMany(p => p.PhysicianPrescrips)
                .HasForeignKey(d => d.PhysicianAdviceId)
                .HasConstraintName("FK_Prescrip_Advice");
        });

        modelBuilder.Entity<PurchaseOrderHeader>(entity =>
        {
            entity.HasKey(e => e.Poid).HasName("PK__Purchase__5F02A2F47E24C1E8");

            entity.ToTable("PurchaseOrderHeader");

            entity.Property(e => e.Poid).HasColumnName("POID");
            entity.Property(e => e.Podate)
                .HasColumnType("datetime")
                .HasColumnName("PODate");
            entity.Property(e => e.Pono)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("PONo");
            entity.Property(e => e.SupplierId).HasColumnName("SupplierID");

            entity.HasOne(d => d.Supplier).WithMany(p => p.PurchaseOrderHeaders)
                .HasForeignKey(d => d.SupplierId)
                .HasConstraintName("FK_PO_Supplier");
        });

        modelBuilder.Entity<PurchaseProductLine>(entity =>
        {
            entity.HasKey(e => e.PolineId).HasName("PK__Purchase__07B9D3425619C648");

            entity.ToTable("PurchaseProductLine");

            entity.Property(e => e.PolineId).HasColumnName("POLineID");
            entity.Property(e => e.DrugId).HasColumnName("DrugID");
            entity.Property(e => e.Note)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Poid).HasColumnName("POID");

            entity.HasOne(d => d.Drug).WithMany(p => p.PurchaseProductLines)
                .HasForeignKey(d => d.DrugId)
                .HasConstraintName("FK_PurchaseLine_Drug");

            entity.HasOne(d => d.Po).WithMany(p => p.PurchaseProductLines)
                .HasForeignKey(d => d.Poid)
                .HasConstraintName("FK_PurchaseLine_PO");
        });

        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.HasKey(e => e.ScheduleId).HasName("PK__Schedule__9C8A5B69A01FC4E3");

            entity.ToTable("Schedule");

            entity.Property(e => e.ScheduleId).HasColumnName("ScheduleID");
            entity.Property(e => e.AppointmentId).HasColumnName("AppointmentID");
            entity.Property(e => e.PhysicianId).HasColumnName("PhysicianID");
            entity.Property(e => e.ScheduleStatus)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ScheduleTime)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.HasOne(d => d.Appointment).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.AppointmentId)
                .HasConstraintName("FK_Schedule_Appointment");

            entity.HasOne(d => d.Physician).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.PhysicianId)
                .HasConstraintName("FK_Schedule_Physician");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.SupplierId).HasName("PK__Supplier__4BE666949E56E28B");

            entity.ToTable("Supplier");

            entity.Property(e => e.SupplierId).HasColumnName("SupplierID");
            entity.Property(e => e.Address)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.SupplierName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.SupplierStatus)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__1788CCACC63841C7");

            entity.ToTable("User");

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.RoleReferenceId).HasColumnName("RoleReferenceID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UserName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
