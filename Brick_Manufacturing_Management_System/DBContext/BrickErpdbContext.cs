using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Brick_Manufacturing_Management_System.Models;

namespace Brick_Manufacturing_Management_System.DBContext;

public partial class BrickErpdbContext : DbContext
{
    public BrickErpdbContext()
    {
    }

    public BrickErpdbContext(DbContextOptions<BrickErpdbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AdvanceDeduction> AdvanceDeductions { get; set; }

    public virtual DbSet<AdvanceType> AdvanceTypes { get; set; }

    public virtual DbSet<BrickProduction> BrickProductions { get; set; }

    public virtual DbSet<BrickSale> BrickSales { get; set; }

    public virtual DbSet<BrickType> BrickTypes { get; set; }

    public virtual DbSet<CustomerMaster> CustomerMasters { get; set; }

    public virtual DbSet<CustomerPayment> CustomerPayments { get; set; }

    public virtual DbSet<LabourAdvance> LabourAdvances { get; set; }

    public virtual DbSet<LabourExpense> LabourExpenses { get; set; }

    public virtual DbSet<LabourMaster> LabourMasters { get; set; }

    public virtual DbSet<LabourWork> LabourWorks { get; set; }

    public virtual DbSet<MaterialMaster> MaterialMasters { get; set; }

    public virtual DbSet<MaterialPurchase> MaterialPurchases { get; set; }

    public virtual DbSet<PaymentMode> PaymentModes { get; set; }

    public virtual DbSet<PaymentStatus> PaymentStatuses { get; set; }

    public virtual DbSet<SalaryPayment> SalaryPayments { get; set; }

    public virtual DbSet<Unit> Units { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<VendorMaster> VendorMasters { get; set; }

    public virtual DbSet<VendorPayment> VendorPayments { get; set; }

    // Keyless result sets for stored procedures
    public DbSet<LabourLedgerResult> LabourLedgerResults { get; set; }
    public DbSet<MaterialReportRow> MaterialReportRows { get; set; }
    public DbSet<SalesReportSpResult> SalesReportSpResults { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=.\\SQLBatch21;Initial Catalog=BrickERPDB;Persist Security Info=True;User ID=sa;Password=Sql123;Encrypt=False;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AdvanceDeduction>(entity =>
        {
            entity.HasKey(e => e.DeductionId).HasName("PK__AdvanceD__E2604C572345C927");

            entity.ToTable("AdvanceDeduction");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Reason).HasMaxLength(200);

            entity.HasOne(d => d.Labour).WithMany(p => p.AdvanceDeductions)
                .HasForeignKey(d => d.LabourId)
                .HasConstraintName("FK__AdvanceDe__Labou__4D94879B");
        });

        modelBuilder.Entity<AdvanceType>(entity =>
        {
            entity.HasKey(e => e.AdvanceTypeId).HasName("PK__AdvanceT__607B08FF851F5352");

            entity.ToTable("AdvanceType");

            entity.Property(e => e.AdvanceTypeName).HasMaxLength(50);
        });

        modelBuilder.Entity<BrickProduction>(entity =>
        {
            entity.HasKey(e => e.ProductionId).HasName("PK__BrickPro__D5D9A2D5EB75ACEE");

            entity.ToTable("BrickProduction");

            entity.HasOne(d => d.BrickType).WithMany(p => p.BrickProductions)
                .HasForeignKey(d => d.BrickTypeId)
                .HasConstraintName("FK_BrickProduction_BrickType");
        });

        modelBuilder.Entity<BrickSale>(entity =>
        {
            entity.HasKey(e => e.SalesId).HasName("PK__BrickSal__C952FB325014C4DC");

            entity.Property(e => e.BrickType).HasMaxLength(50);
            entity.Property(e => e.PaidAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PendingAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Rate).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Customer).WithMany(p => p.BrickSales)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__BrickSale__Custo__571DF1D5");
        });

        modelBuilder.Entity<BrickType>(entity =>
        {
            entity.HasKey(e => e.BrickTypeId).HasName("PK__BrickTyp__EB084672F3218727");

            entity.ToTable("BrickType");

            entity.Property(e => e.BrickTypeName).HasMaxLength(100);
            entity.Property(e => e.Status).HasDefaultValue(true);
        });

        modelBuilder.Entity<CustomerMaster>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__Customer__A4AE64D871FD4842");

            entity.ToTable("CustomerMaster");

            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.CustomerName).HasMaxLength(100);
            entity.Property(e => e.MobileNumber).HasMaxLength(15);
        });

        modelBuilder.Entity<CustomerPayment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Customer__9B556A380E29DBE6");

            entity.ToTable("CustomerPayment");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Customer).WithMany(p => p.CustomerPayments)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__CustomerP__Custo__59FA5E80");

            entity.HasOne(d => d.PaymentMode).WithMany(p => p.CustomerPayments)
                .HasForeignKey(d => d.PaymentModeId)
                .HasConstraintName("FK_CustomerPayment_Mode");

            entity.HasOne(d => d.Status).WithMany(p => p.CustomerPayments)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK_CustomerPayment_Status");
        });

        modelBuilder.Entity<LabourAdvance>(entity =>
        {
            entity.HasKey(e => e.AdvanceId).HasName("PK__LabourAd__27C9D2A0C25762A5");

            entity.ToTable("LabourAdvance");

            entity.Property(e => e.AdvanceAmount).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.AdvanceType).WithMany(p => p.LabourAdvances)
                .HasForeignKey(d => d.AdvanceTypeId)
                .HasConstraintName("FK_LabourAdvance_Type");

            entity.HasOne(d => d.Labour).WithMany(p => p.LabourAdvances)
                .HasForeignKey(d => d.LabourId)
                .HasConstraintName("FK__LabourAdv__Labou__44FF419A");
        });

        modelBuilder.Entity<LabourExpense>(entity =>
        {
            entity.HasKey(e => e.ExpenseId).HasName("PK__LabourEx__1445CFD3727F9BF1");

            entity.ToTable("LabourExpense");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Reason).HasMaxLength(200);

            entity.HasOne(d => d.Labour).WithMany(p => p.LabourExpenses)
                .HasForeignKey(d => d.LabourId)
                .HasConstraintName("FK__LabourExp__Labou__4AB81AF0");
        });

        modelBuilder.Entity<LabourMaster>(entity =>
        {
            entity.HasKey(e => e.LabourId).HasName("PK__LabourMa__74A963F749FF0F81");

            entity.ToTable("LabourMaster");

            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.DailyWage).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.LabourName).HasMaxLength(100);
            entity.Property(e => e.MobileNumber).HasMaxLength(15);
        });

        modelBuilder.Entity<LabourWork>(entity =>
        {
            entity.HasKey(e => e.WorkId).HasName("PK__LabourWo__2DE6D5F51801F2AE");

            entity.ToTable("LabourWork");

            entity.Property(e => e.DailyWage).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalSalary).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Labour).WithMany(p => p.LabourWorks)
                .HasForeignKey(d => d.LabourId)
                .HasConstraintName("FK__LabourWor__Labou__47DBAE45");
        });

        modelBuilder.Entity<MaterialMaster>(entity =>
        {
            entity.HasKey(e => e.MaterialId).HasName("PK__Material__C50610F78A07DF4F");

            entity.ToTable("MaterialMaster");

            entity.Property(e => e.MaterialName).HasMaxLength(100);
            entity.Property(e => e.Status).HasDefaultValue(true);

            entity.HasOne(d => d.Unit).WithMany(p => p.MaterialMasters)
                .HasForeignKey(d => d.UnitId)
                .HasConstraintName("FK_Material_Unit");
        });

        modelBuilder.Entity<MaterialPurchase>(entity =>
        {
            entity.HasKey(e => e.PurchaseId).HasName("PK__Material__6B0A6BBEAAF23119");

            entity.ToTable("MaterialPurchase");

            entity.Property(e => e.PaidAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PendingAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Quantity).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Rate).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Material).WithMany(p => p.MaterialPurchases)
                .HasForeignKey(d => d.MaterialId)
                .HasConstraintName("FK__MaterialP__Mater__403A8C7D");

            entity.HasOne(d => d.Vendor).WithMany(p => p.MaterialPurchases)
                .HasForeignKey(d => d.VendorId)
                .HasConstraintName("FK__MaterialP__Vendo__3F466844");
        });

        modelBuilder.Entity<PaymentMode>(entity =>
        {
            entity.HasKey(e => e.PaymentModeId).HasName("PK__PaymentM__F95995492574DA34");

            entity.ToTable("PaymentMode");

            entity.Property(e => e.PaymentModeName).HasMaxLength(50);
            entity.Property(e => e.Status).HasDefaultValue(true);
        });

        modelBuilder.Entity<PaymentStatus>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__PaymentS__C8EE206372F64CAD");

            entity.ToTable("PaymentStatus");

            entity.Property(e => e.StatusName).HasMaxLength(20);
        });

        modelBuilder.Entity<SalaryPayment>(entity =>
        {
            entity.HasKey(e => e.SalaryId).HasName("PK__SalaryPa__4BE204573D4F4EF2");

            entity.ToTable("SalaryPayment");

            entity.Property(e => e.FinalSalary).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PaidAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalExpense).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalSalary).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Labour).WithMany(p => p.SalaryPayments)
                .HasForeignKey(d => d.LabourId)
                .HasConstraintName("FK__SalaryPay__Labou__5070F446");

            entity.HasOne(d => d.Status).WithMany(p => p.SalaryPayments)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK_SalaryPayment_Status");
        });

        modelBuilder.Entity<Unit>(entity =>
        {
            entity.HasKey(e => e.UnitId).HasName("PK__Units__44F5ECB51B8020B5");

            entity.Property(e => e.UnitName).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4CC3247698");

            entity.Property(e => e.Password).HasMaxLength(100);
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        modelBuilder.Entity<VendorMaster>(entity =>
        {
            entity.HasKey(e => e.VendorId).HasName("PK__VendorMa__FC8618F380145E6A");

            entity.ToTable("VendorMaster");

            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.Gstnumber)
                .HasMaxLength(20)
                .HasColumnName("GSTNumber");
            entity.Property(e => e.MobileNumber).HasMaxLength(15);
            entity.Property(e => e.Status).HasDefaultValue(true);
            entity.Property(e => e.VendorName).HasMaxLength(100);
        });

        modelBuilder.Entity<VendorPayment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__VendorPa__9B556A382A4B0E9C");

            entity.ToTable("VendorPayment");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PaymentDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.PaymentMode).WithMany(p => p.VendorPayments)
                .HasForeignKey(d => d.PaymentModeId)
                .HasConstraintName("FK__VendorPay__Payme__17F790F9");

            entity.HasOne(d => d.Status).WithMany(p => p.VendorPayments)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__VendorPay__Statu__18EBB532");

            entity.HasOne(d => d.Vendor).WithMany(p => p.VendorPayments)
                .HasForeignKey(d => d.VendorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__VendorPay__Vendo__17036CC0");
        });

        modelBuilder.Entity<LabourLedgerResult>().HasNoKey().ToView(null);
        modelBuilder.Entity<MaterialReportRow>().HasNoKey().ToView(null);
        modelBuilder.Entity<SalesReportSpResult>().HasNoKey().ToView(null);

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
