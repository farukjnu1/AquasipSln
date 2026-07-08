using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Aquasip.EF;

public partial class AquasipContext : DbContext
{
    public AquasipContext()
    {
    }

    public AquasipContext(DbContextOptions<AquasipContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<ContactMessage> ContactMessages { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<CustomerPayment> CustomerPayments { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Medium> Media { get; set; }

    public virtual DbSet<Menu> Menus { get; set; }

    public virtual DbSet<Page> Pages { get; set; }

    public virtual DbSet<PageContent> PageContents { get; set; }

    public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }

    public virtual DbSet<PaymentStatus> PaymentStatuses { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductMedium> ProductMedia { get; set; }

    public virtual DbSet<ProductPrice> ProductPrices { get; set; }

    public virtual DbSet<ProductRatingSummary> ProductRatingSummaries { get; set; }

    public virtual DbSet<PurchaseOrder> PurchaseOrders { get; set; }

    public virtual DbSet<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }

    public virtual DbSet<PurchaseOrderState> PurchaseOrderStates { get; set; }

    public virtual DbSet<PurchaseReturn> PurchaseReturns { get; set; }

    public virtual DbSet<PurchaseReturnDetail> PurchaseReturnDetails { get; set; }

    public virtual DbSet<ReferenceType> ReferenceTypes { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<ReviewComment> ReviewComments { get; set; }

    public virtual DbSet<ReviewMedium> ReviewMedia { get; set; }

    public virtual DbSet<ReviewVote> ReviewVotes { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RolePermission> RolePermissions { get; set; }

    public virtual DbSet<SalesOrder> SalesOrders { get; set; }

    public virtual DbSet<SalesOrderDetail> SalesOrderDetails { get; set; }

    public virtual DbSet<SalesOrderState> SalesOrderStates { get; set; }

    public virtual DbSet<SalesReturn> SalesReturns { get; set; }

    public virtual DbSet<SalesReturnDetail> SalesReturnDetails { get; set; }

    public virtual DbSet<ShippingAddress> ShippingAddresses { get; set; }

    public virtual DbSet<SiteSetting> SiteSettings { get; set; }

    public virtual DbSet<StockTransaction> StockTransactions { get; set; }

    public virtual DbSet<Store> Stores { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<SupplierPayment> SupplierPayments { get; set; }

    public virtual DbSet<TransactionType> TransactionTypes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=Faruk-Abdullah;Database=Aquasip;User=sa;Password=123;TrustServerCertificate=True;Trusted_Connection=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.ToTable("Appointment");

            entity.Property(e => e.Address).HasMaxLength(510);
            entity.Property(e => e.Age).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.AppointmentDate).HasColumnType("datetime");
            entity.Property(e => e.CreateAt).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FullName).HasMaxLength(255);
            entity.Property(e => e.Phone)
                .HasMaxLength(25)
                .IsUnicode(false);
            entity.Property(e => e.ProblemDetail).HasMaxLength(1020);
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__Comments__C3B4DFAA5E45F07C");

            entity.Property(e => e.CommentId).HasColumnName("CommentID");
            entity.Property(e => e.AuthorEmail).HasMaxLength(100);
            entity.Property(e => e.AuthorName).HasMaxLength(100);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PageId).HasColumnName("PageID");

            entity.HasOne(d => d.Page).WithMany(p => p.Comments)
                .HasForeignKey(d => d.PageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Comments__PageID__59063A47");
        });

        modelBuilder.Entity<ContactMessage>(entity =>
        {
            entity.ToTable("ContactMessage");

            entity.Property(e => e.CreateAt).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FullName).HasMaxLength(255);
            entity.Property(e => e.Phone)
                .HasMaxLength(25)
                .IsUnicode(false);
            entity.Property(e => e.Subject).HasMaxLength(510);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__Customer__1788CC4CF5FB3A7F");

            entity.ToTable("Customer");

            entity.HasIndex(e => e.Email, "UQ__Customer__A9D105340BF65E6B").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.CustomerCode).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.FullName).HasMaxLength(200);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(30);
        });

        modelBuilder.Entity<CustomerPayment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__PaymentT__C22AEFE0547916C4");

            entity.ToTable("CustomerPayment");

            entity.HasIndex(e => e.OrderId, "IX_PaymentTransactions_OrderId");

            entity.Property(e => e.PaidAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PaymentStatusId).HasDefaultValue(1);
            entity.Property(e => e.Remarks).HasMaxLength(500);
            entity.Property(e => e.TransactionNumber).HasMaxLength(100);

            entity.HasOne(d => d.Order).WithMany(p => p.CustomerPayments)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CustomerPayment_SalesOrder");

            entity.HasOne(d => d.PaymentMethod).WithMany(p => p.CustomerPayments)
                .HasForeignKey(d => d.PaymentMethodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CustomerPayment_PaymentMethods");

            entity.HasOne(d => d.PaymentStatus).WithMany(p => p.CustomerPayments)
                .HasForeignKey(d => d.PaymentStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CustomerPayment_PaymentStatus");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.ToTable("Department");

            entity.Property(e => e.Code)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Description).HasMaxLength(510);
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<Medium>(entity =>
        {
            entity.HasKey(e => e.MediaId).HasName("PK__Media__B2C2B5AFDE58E0E5");

            entity.Property(e => e.MediaId).HasColumnName("MediaID");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.FileName).HasMaxLength(255);
            entity.Property(e => e.FilePath).HasMaxLength(255);
            entity.Property(e => e.UploadedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.UploadedByNavigation).WithMany(p => p.Media)
                .HasForeignKey(d => d.UploadedBy)
                .HasConstraintName("FK__Media__UploadedB__45F365D3");
        });

        modelBuilder.Entity<Menu>(entity =>
        {
            entity.HasKey(e => e.MenuId).HasName("PK__Menus__C99ED250FAA4E141");

            entity.Property(e => e.MenuId).HasColumnName("MenuID");
            entity.Property(e => e.Label).HasMaxLength(100);
            entity.Property(e => e.PageId).HasColumnName("PageID");
            entity.Property(e => e.ParentId).HasColumnName("ParentID");

            entity.HasOne(d => d.Page).WithMany(p => p.Menus)
                .HasForeignKey(d => d.PageId)
                .HasConstraintName("FK__Menus__PageID__5AEE82B9");

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("FK__Menus__ParentID__5BE2A6F2");
        });

        modelBuilder.Entity<Page>(entity =>
        {
            entity.HasKey(e => e.PageId).HasName("PK__Pages__C565B12425CF940C");

            entity.HasIndex(e => e.Slug, "UQ__Pages__BC7B5FB6904CF83B").IsUnique();

            entity.Property(e => e.PageId).HasColumnName("PageID");
            entity.Property(e => e.AuthorId).HasColumnName("AuthorID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PublishedAt).HasColumnType("datetime");
            entity.Property(e => e.Slug).HasMaxLength(255);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("draft");
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.Author).WithMany(p => p.Pages)
                .HasForeignKey(d => d.AuthorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Pages__AuthorID__5CD6CB2B");
        });

        modelBuilder.Entity<PageContent>(entity =>
        {
            entity.HasKey(e => e.PageContentId).HasName("PK_PageContent");

            entity.Property(e => e.SlugPage).HasMaxLength(255);
            entity.Property(e => e.SlugPageContent).HasMaxLength(255);
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.UploadedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasKey(e => e.PaymentMethodId).HasName("PK__PaymentM__DC31C1D32E2EC19A");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PaymentMethodName).HasMaxLength(100);
        });

        modelBuilder.Entity<PaymentStatus>(entity =>
        {
            entity.HasKey(e => e.PaymentStateId);

            entity.ToTable("PaymentStatus");

            entity.Property(e => e.PaymentStateId).ValueGeneratedNever();
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PaymentStatus1)
                .HasMaxLength(50)
                .HasColumnName("PaymentStatus");
            entity.Property(e => e.Remark).HasMaxLength(100);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Products__B40CC6CDBDE63684");

            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ProductCode).HasMaxLength(20);
            entity.Property(e => e.ProductName).HasMaxLength(200);
            entity.Property(e => e.UploadedAt).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<ProductMedium>(entity =>
        {
            entity.HasKey(e => e.ProductMediaId);

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.FileName).HasMaxLength(255);
            entity.Property(e => e.FilePath).HasMaxLength(255);
            entity.Property(e => e.UploadedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<ProductPrice>(entity =>
        {
            entity.ToTable("ProductPrice");

            entity.Property(e => e.Description).HasMaxLength(250);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UploadedAt).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<ProductRatingSummary>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__ProductR__B40CC6CD1D389296");

            entity.ToTable("ProductRatingSummary");

            entity.Property(e => e.ProductId).ValueGeneratedNever();
            entity.Property(e => e.AverageRating).HasColumnType("decimal(3, 2)");
            entity.Property(e => e.LastUpdated).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.TotalReviews).HasDefaultValue(0);

            entity.HasOne(d => d.Product).WithOne(p => p.ProductRatingSummary)
                .HasForeignKey<ProductRatingSummary>(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Summary_Product");
        });

        modelBuilder.Entity<PurchaseOrder>(entity =>
        {
            entity.HasKey(e => e.PurchaseOrderId).HasName("PK__Purchase__036BACA406E7EFE7");

            entity.ToTable("PurchaseOrder");

            entity.HasIndex(e => e.Ponumber, "UQ__Purchase__69B9A84165F4B915").IsUnique();

            entity.Property(e => e.DiscountAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.OtherCharge).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Podate)
                .HasColumnType("datetime")
                .HasColumnName("PODate");
            entity.Property(e => e.Ponumber)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("PONumber");
            entity.Property(e => e.Remark).HasMaxLength(500);
            entity.Property(e => e.SubTotal).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TaxAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TaxPercent).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.PurchaseState).WithMany(p => p.PurchaseOrders)
                .HasForeignKey(d => d.PurchaseStateId)
                .HasConstraintName("FK_PurchaseOrder_PurchaseOrderState");

            entity.HasOne(d => d.Supplier).WithMany(p => p.PurchaseOrders)
                .HasForeignKey(d => d.SupplierId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PurchaseO__Suppl__1B9317B3");
        });

        modelBuilder.Entity<PurchaseOrderDetail>(entity =>
        {
            entity.HasKey(e => e.PurchaseOrderDetailId).HasName("PK__Purchase__5026B698719BBE57");

            entity.ToTable("PurchaseOrderDetail");

            entity.Property(e => e.DiscountAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.LineTotal).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Qty).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UnitCost).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Product).WithMany(p => p.PurchaseOrderDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PurchaseO__Produ__2334397B");

            entity.HasOne(d => d.PurchaseOrder).WithMany(p => p.PurchaseOrderDetails)
                .HasForeignKey(d => d.PurchaseOrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PurchaseO__Purch__22401542");
        });

        modelBuilder.Entity<PurchaseOrderState>(entity =>
        {
            entity.HasKey(e => e.PurchaseStateId);

            entity.ToTable("PurchaseOrderState");

            entity.Property(e => e.PurchaseStateId).ValueGeneratedNever();
            entity.Property(e => e.ColorCode).HasMaxLength(20);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PurchaseStatus).HasMaxLength(50);
            entity.Property(e => e.Remark).HasMaxLength(100);
        });

        modelBuilder.Entity<PurchaseReturn>(entity =>
        {
            entity.HasKey(e => e.PurchaseReturnId).HasName("PK__Purchase__2C33C6E87D773480");

            entity.ToTable("PurchaseReturn");

            entity.HasIndex(e => e.ReturnNumber, "UQ__Purchase__2739D7BBA1037D70").IsUnique();

            entity.Property(e => e.ReturnDate).HasColumnType("datetime");
            entity.Property(e => e.ReturnNumber)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.PurchaseOrder).WithMany(p => p.PurchaseReturns)
                .HasForeignKey(d => d.PurchaseOrderId)
                .HasConstraintName("FK__PurchaseR__Purch__27F8EE98");

            entity.HasOne(d => d.Supplier).WithMany(p => p.PurchaseReturns)
                .HasForeignKey(d => d.SupplierId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PurchaseR__Suppl__2704CA5F");
        });

        modelBuilder.Entity<PurchaseReturnDetail>(entity =>
        {
            entity.HasKey(e => e.PurchaseReturnDetailId).HasName("PK__Purchase__4CB914C590E96DF9");

            entity.ToTable("PurchaseReturnDetail");

            entity.Property(e => e.LineTotal).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Qty).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UnitCost).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Product).WithMany(p => p.PurchaseReturnDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PurchaseR__Produ__2F9A1060");

            entity.HasOne(d => d.PurchaseReturn).WithMany(p => p.PurchaseReturnDetails)
                .HasForeignKey(d => d.PurchaseReturnId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PurchaseR__Purch__2EA5EC27");
        });

        modelBuilder.Entity<ReferenceType>(entity =>
        {
            entity.ToTable("ReferenceType");

            entity.Property(e => e.ReferenceTypeId).ValueGeneratedNever();
            entity.Property(e => e.Code)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PK__Reviews__74BC79CE355DC824");

            entity.HasIndex(e => e.CustomerId, "IX_Reviews_CustomerId");

            entity.HasIndex(e => e.ProductId, "IX_Reviews_ProductId");

            entity.HasIndex(e => new { e.ProductId, e.CustomerId }, "UQ_Product_Customer").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsApproved).HasDefaultValue(true);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.ModerationStatus)
                .HasMaxLength(50)
                .HasDefaultValue("Approved");
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.Customer).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reviews_Customer");

            entity.HasOne(d => d.Product).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reviews_Product");
        });

        modelBuilder.Entity<ReviewComment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__ReviewCo__C3B4DFCAE956CF55");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Customer).WithMany(p => p.ReviewComments)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Comments_Customer");

            entity.HasOne(d => d.Review).WithMany(p => p.ReviewComments)
                .HasForeignKey(d => d.ReviewId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Comments_Review");
        });

        modelBuilder.Entity<ReviewMedium>(entity =>
        {
            entity.HasKey(e => e.MediaId).HasName("PK__ReviewMe__B2C2B5CF2A6FE60C");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.MediaType).HasMaxLength(50);
            entity.Property(e => e.MediaUrl).HasMaxLength(500);

            entity.HasOne(d => d.Review).WithMany(p => p.ReviewMedia)
                .HasForeignKey(d => d.ReviewId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Media_Review");
        });

        modelBuilder.Entity<ReviewVote>(entity =>
        {
            entity.HasKey(e => e.VoteId).HasName("PK__ReviewVo__52F015C23DAAE953");

            entity.HasIndex(e => e.ReviewId, "IX_ReviewVotes_ReviewId");

            entity.HasIndex(e => new { e.ReviewId, e.CustomerId }, "UQ_Review_Customer_Vote").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Customer).WithMany(p => p.ReviewVotes)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Votes_Customer");

            entity.HasOne(d => d.Review).WithMany(p => p.ReviewVotes)
                .HasForeignKey(d => d.ReviewId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Votes_Review");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE3A2568E5D7");

            entity.HasIndex(e => e.RoleName, "UQ__Roles__8A2B61605BB8AE29").IsUnique();

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.ToTable("RolePermission");

            entity.Property(e => e.Controller)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<SalesOrder>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__C3905BCFD1FCEEB7");

            entity.ToTable("SalesOrder");

            entity.HasIndex(e => e.OrderNumber, "IX_Orders_OrderNumber");

            entity.HasIndex(e => e.OrderNumber, "UQ__Orders__CAC5E743E8E86ECE").IsUnique();

            entity.Property(e => e.DeliveryCharge).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.GatewayCharge).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.GrandTotal).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.OrderDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.OrderNumber).HasMaxLength(50);
            entity.Property(e => e.OrderStateId).HasDefaultValue(1);
            entity.Property(e => e.SubTotal).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.VatAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.VatPercent).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.Customer).WithMany(p => p.SalesOrders)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_Customers");

            entity.HasOne(d => d.PaymentMethod).WithMany(p => p.SalesOrders)
                .HasForeignKey(d => d.PaymentMethodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_PaymentMethods");

            entity.HasOne(d => d.ShippingAddress).WithMany(p => p.SalesOrders)
                .HasForeignKey(d => d.ShippingAddressId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_ShippingAddresses");
        });

        modelBuilder.Entity<SalesOrderDetail>(entity =>
        {
            entity.HasKey(e => e.OrderDetailId).HasName("PK__OrderDet__D3B9D36C29543BE6");

            entity.HasIndex(e => e.OrderId, "IX_OrderDetails_OrderId");

            entity.HasIndex(e => e.ProductId, "IX_OrderDetails_ProductId");

            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Order).WithMany(p => p.SalesOrderDetails)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderDetails_Orders");

            entity.HasOne(d => d.Product).WithMany(p => p.SalesOrderDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderDetails_Products");
        });

        modelBuilder.Entity<SalesOrderState>(entity =>
        {
            entity.HasKey(e => e.OrderStateId).HasName("PK_OrderStates");

            entity.Property(e => e.OrderStateId).ValueGeneratedNever();
            entity.Property(e => e.ColorCode).HasMaxLength(20);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.OrderStatus).HasMaxLength(50);
            entity.Property(e => e.Remark).HasMaxLength(100);
        });

        modelBuilder.Entity<SalesReturn>(entity =>
        {
            entity.HasKey(e => e.SalesReturnId).HasName("PK__SalesRet__E0906C383382472F");

            entity.ToTable("SalesReturn");

            entity.HasIndex(e => e.ReturnNumber, "UQ__SalesRet__2739D7BBADD6E687").IsUnique();

            entity.Property(e => e.ReturnDate).HasColumnType("datetime");
            entity.Property(e => e.ReturnNumber)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Customer).WithMany(p => p.SalesReturns)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SalesRetu__Custo__0F2D40CE");

            entity.HasOne(d => d.SalesOrder).WithMany(p => p.SalesReturns)
                .HasForeignKey(d => d.SalesOrderId)
                .HasConstraintName("FK__SalesRetu__Sales__10216507");
        });

        modelBuilder.Entity<SalesReturnDetail>(entity =>
        {
            entity.HasKey(e => e.SalesReturnDetailId).HasName("PK__SalesRet__B6BCC9488BE9C713");

            entity.ToTable("SalesReturnDetail");

            entity.Property(e => e.LineTotal).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Qty).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Product).WithMany(p => p.SalesReturnDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SalesRetu__Produ__13F1F5EB");

            entity.HasOne(d => d.SalesReturn).WithMany(p => p.SalesReturnDetails)
                .HasForeignKey(d => d.SalesReturnId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SalesRetu__Sales__12FDD1B2");
        });

        modelBuilder.Entity<ShippingAddress>(entity =>
        {
            entity.HasKey(e => e.ShippingAddressId).HasName("PK__Shipping__EC10DC39668204E2");

            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.CountryCode).HasMaxLength(5);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.EmailAddress).HasMaxLength(255);
            entity.Property(e => e.FullName).HasMaxLength(250);
            entity.Property(e => e.PhoneNumber).HasMaxLength(30);
            entity.Property(e => e.PostalCode).HasMaxLength(20);
            entity.Property(e => e.StateProvince).HasMaxLength(100);
            entity.Property(e => e.StreetAddress).HasMaxLength(500);

            entity.HasOne(d => d.Customer).WithMany(p => p.ShippingAddresses)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ShippingAddresses_Customers");
        });

        modelBuilder.Entity<SiteSetting>(entity =>
        {
            entity.HasKey(e => e.SettingKey).HasName("PK__SiteSett__01E719AC6C3C4212");

            entity.Property(e => e.SettingKey).HasMaxLength(100);
        });

        modelBuilder.Entity<StockTransaction>(entity =>
        {
            entity.HasKey(e => e.StockTransactionId).HasName("PK__StockTra__C6E6C5CA3C3AACCF");

            entity.ToTable("StockTransaction");

            entity.Property(e => e.QtyIn)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.QtyOut)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TransactionDate).HasColumnType("datetime");
            entity.Property(e => e.UnitCost).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Product).WithMany(p => p.StockTransactions)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StockTran__Produ__345EC57D");
        });

        modelBuilder.Entity<Store>(entity =>
        {
            entity.ToTable("Store");

            entity.Property(e => e.StoreCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.StoreName).HasMaxLength(50);
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.SupplierId).HasName("PK__Supplier__4BE666B4AD590C23");

            entity.ToTable("Supplier");

            entity.HasIndex(e => e.SupplierCode, "UQ__Supplier__44BE981B8C9FD968").IsUnique();

            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Phone)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.SupplierCode)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.SupplierName)
                .HasMaxLength(200)
                .IsUnicode(false);
        });

        modelBuilder.Entity<SupplierPayment>(entity =>
        {
            entity.HasKey(e => e.PaymentId);

            entity.ToTable("SupplierPayment");

            entity.Property(e => e.PaidAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PaymentStatusId).HasDefaultValue(1);
            entity.Property(e => e.Remarks).HasMaxLength(500);
            entity.Property(e => e.TransactionNumber).HasMaxLength(100);

            entity.HasOne(d => d.PaymentMethod).WithMany(p => p.SupplierPayments)
                .HasForeignKey(d => d.PaymentMethodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SupplierPayment_PaymentMethods");

            entity.HasOne(d => d.PaymentStatus).WithMany(p => p.SupplierPayments)
                .HasForeignKey(d => d.PaymentStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SupplierPayment_PaymentStatus");

            entity.HasOne(d => d.PurchaseOrder).WithMany(p => p.SupplierPayments)
                .HasForeignKey(d => d.PurchaseOrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SupplierPayment_SupplierPayment");
        });

        modelBuilder.Entity<TransactionType>(entity =>
        {
            entity.ToTable("TransactionType");

            entity.Property(e => e.TransactionTypeId).ValueGeneratedNever();
            entity.Property(e => e.Code)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Name).HasMaxLength(20);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCACE7491250");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E42D590F47").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534365D8D8F").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.UserRoleId).HasName("PK__UserRole__AF27604F28CA2AED");

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserRoles__RoleI__5812160E");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserRoles__UserI__571DF1D5");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
