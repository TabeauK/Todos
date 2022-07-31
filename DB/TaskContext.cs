using System.Data.Entity;
using System.Collections.Generic;

namespace Todos
{
    public class TaskContext : DbContext
    {
        public TaskContext() : base(Constants.ConnectionString)
        {
            Database.SetInitializer(new TaskDBInit());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new TaskConfiguration());
        }

        public DbSet<Task> Tasks { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Check> Checks { get; set; }

        public class TaskDBInit : CreateDatabaseIfNotExists <TaskContext>
        { 
            protected override void Seed(TaskContext context)
            {
                context.Users.AddRange(new List<User> {
                    new User {
                        Id = 1,
                        UserName = "Rodzinne"
                    },
                    new User {
                        Id = 2,
                        UserName = "Krzyś"
                    },
                    new User {
                        Id = 3,
                        UserName = "Ada"
                    }});
                base.Seed(context);
            }
        }
    }
}
