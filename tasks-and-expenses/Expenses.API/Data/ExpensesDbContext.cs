using Microsoft.EntityFrameworkCore;
using Expenses.API.Models;

namespace Expenses.API.Data;

public class ExpensesDbContext : DbContext
{
    public ExpensesDbContext(DbContextOptions<ExpensesDbContext> options) : base(options)
    {
    }

    public DbSet<Expense> Expenses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("expenses");

        modelBuilder.Entity<Expense>(entity =>
        {
            entity.ToTable("expense");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.UserId).HasColumnName("user_id").IsRequired();
            entity.Property(e => e.Expenses)
                .HasColumnName("expenses")
                .HasColumnType("jsonb")
                .HasDefaultValue("[]");
            entity.Property(e => e.PreferredCurrency)
                .HasColumnName("preferredCurrency")
                .HasMaxLength(3);
            entity.Property(e => e.MonthlySalary)
                .HasColumnName("monthlySalary")
                .HasColumnType("decimal(12,2)");
            entity.Property(e => e.SalaryInputCurrency)
                .HasColumnName("salaryInputCurrency")
                .HasMaxLength(3);
            entity.Property(e => e.IdealSavings)
                .HasColumnName("idealSavings")
                .HasColumnType("decimal(12,2)");
            entity.Property(e => e.SavingsInputCurrency)
                .HasColumnName("savingsInputCurrency")
                .HasMaxLength(3);
            entity.Property(e => e.IdealMonthlyExpenses)
                .HasColumnName("idealMonthlyExpenses")
                .HasColumnType("decimal(12,2)");
            entity.Property(e => e.ExchangeRateBuy)
                .HasColumnName("exchangeRateBuy")
                .HasColumnType("decimal(12,4)");
            entity.Property(e => e.ExchangeRateSell)
                .HasColumnName("exchangeRateSell")
                .HasColumnType("decimal(12,4)");
            entity.Property(e => e.Month)
                .HasColumnName("month")
                .IsRequired();
            entity.Property(e => e.Year)
                .HasColumnName("year")
                .IsRequired();
            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("NOW()");
            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at")
                .HasDefaultValueSql("NOW()");
        });
    }
}
