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

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Medium> Media { get; set; }

    public virtual DbSet<Menu> Menus { get; set; }

    public virtual DbSet<Page> Pages { get; set; }

    public virtual DbSet<PageContent> PageContents { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductMedium> ProductMedia { get; set; }

    public virtual DbSet<ProductPrice> ProductPrices { get; set; }

    public virtual DbSet<ProductRatingSummary> ProductRatingSummaries { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<ReviewComment> ReviewComments { get; set; }

    public virtual DbSet<ReviewMedium> ReviewMedia { get; set; }

    public virtual DbSet<ReviewVote> ReviewVotes { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RolePermission> RolePermissions { get; set; }

    public virtual DbSet<SiteSetting> SiteSettings { get; set; }

    public virtual DbSet<Specialist> Specialists { get; set; }

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
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
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

        modelBuilder.Entity<SiteSetting>(entity =>
        {
            entity.HasKey(e => e.SettingKey).HasName("PK__SiteSett__01E719AC6C3C4212");

            entity.Property(e => e.SettingKey).HasMaxLength(100);
        });

        modelBuilder.Entity<Specialist>(entity =>
        {
            entity.ToTable("Specialist");

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Designation).HasMaxLength(255);
            entity.Property(e => e.FullName).HasMaxLength(255);
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
