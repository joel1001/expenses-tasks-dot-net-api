using Microsoft.EntityFrameworkCore;
using TaskModel = Tasks.API.Models.Task;

namespace Tasks.API.Data;

public class TasksDbContext : DbContext
{
    public TasksDbContext(DbContextOptions<TasksDbContext> options) : base(options)
    {
    }

    public DbSet<TaskModel> Tasks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TaskModel>(entity =>
        {
            entity.ToTable("task");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.UserId).HasColumnName("user_id").IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
            entity.Property(e => e.CompletedTasks)
                .HasColumnName("completedTasks")
                .HasColumnType("jsonb")
                .HasDefaultValue("[]");
            entity.Property(e => e.Tasks)
                .HasColumnName("tasks")
                .HasColumnType("jsonb")
                .HasDefaultValue("[]");
        });
    }
}

