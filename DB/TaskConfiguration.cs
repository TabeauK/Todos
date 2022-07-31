using System.Data.Entity.ModelConfiguration;

namespace Todos
{
    public class TaskConfiguration : EntityTypeConfiguration<Task>
    {
        public TaskConfiguration()
        {
            this.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(s => s.Interval)
                .IsRequired();
        }
    }
}
