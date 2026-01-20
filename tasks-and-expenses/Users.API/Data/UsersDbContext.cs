using Microsoft.EntityFrameworkCore;
using Users.API.Application.Interfaces;
using Users.API.Models;

namespace Users.API.Data;

public class UsersDbContext : DbContext, IUsersDbContext
{
    public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<CreditCard> CreditCards { get; set; }
    
    DbSet<User> IUsersDbContext.Users => Users;
    
    DbSet<T> IUsersDbContext.Set<T>() where T : class => base.Set<T>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("user");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .UseIdentityColumn();
            entity.Property(e => e.FirstName).HasColumnName("first_name").HasMaxLength(255);
            entity.Property(e => e.LastName).HasColumnName("last_name").HasMaxLength(255);
            entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(255);
            entity.Property(e => e.Password).HasColumnName("password").HasMaxLength(255); // Campo password (almacena el hash)
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash").HasMaxLength(255); // Campo password_hash
            entity.Property(e => e.Phone).HasColumnName("phone").HasMaxLength(50);
            entity.Property(e => e.DateOfBirth)
                .HasColumnName("date_of_birth")
                .HasColumnType("date"); // Tipo date en PostgreSQL (sin hora)
            entity.Property(e => e.CreatedDate)
                .HasColumnName("created_date")
                .HasConversion(
                    v => v.HasValue ? (v.Value.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v.Value.ToUniversalTime()) : (DateTime?)null,
                    v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : (DateTime?)null);
            entity.Property(e => e.UpdatedDate)
                .HasColumnName("updated_date")
                .HasColumnType("timestamp with time zone")
                .HasConversion(
                    v => v.HasValue ? (v.Value.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v.Value.ToUniversalTime()) : (DateTime?)null,
                    v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : (DateTime?)null);
            entity.Property(e => e.HaveCreditCards).HasColumnName("haveCreditCards");
            entity.Property(e => e.HaveLoans).HasColumnName("haveLoans");
            entity.Property(e => e.Token).HasColumnName("token").HasMaxLength(255);
            entity.Property(e => e.TouringStatus)
                .HasColumnName("touringStatus")
                .HasColumnType("numeric");
            entity.Property(e => e.Role)
                .HasColumnName("role")
                .HasMaxLength(50);
            entity.Property(e => e.PreferredCurrency)
                .HasColumnName("preferredCurrency")
                .HasMaxLength(3);
        });

        modelBuilder.Entity<CreditCard>(entity =>
        {
            entity.ToTable("credit_card");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .UseIdentityColumn();
            entity.Property(e => e.UserId)
                .HasColumnName("user_id")
                .IsRequired();
            entity.Property(e => e.Name)
                .HasColumnName("name")
                .HasMaxLength(255)
                .IsRequired();
            entity.Property(e => e.Type)
                .HasColumnName("type")
                .HasMaxLength(20)
                .IsRequired();
            entity.Property(e => e.CutDay)
                .HasColumnName("cut_day");
            entity.Property(e => e.PaymentDay)
                .HasColumnName("payment_day");
            entity.Property(e => e.CreatedDate)
                .HasColumnName("created_date")
                .HasConversion(
                    v => v.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(v, DateTimeKind.Utc) : v.ToUniversalTime(),
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
            entity.Property(e => e.UpdatedDate)
                .HasColumnName("updated_date")
                .HasConversion(
                    v => v.HasValue ? (v.Value.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v.Value.ToUniversalTime()) : (DateTime?)null,
                    v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : (DateTime?)null);
            
            // Foreign key relationship
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}

