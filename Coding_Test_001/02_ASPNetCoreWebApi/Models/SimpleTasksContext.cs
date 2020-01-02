using Microsoft.EntityFrameworkCore;

namespace ASPNetCoreWebApi.Models
{
    public class SimpleTasksContext : DbContext
    {
        // ToDo: Determine if there is an additional library for .NET Core Entity Framework for the use of Interfaces (long a problem with the typical response being something like the answer here: https://github.com/aspnet/EntityFrameworkCore/issues/15358

        public SimpleTasksContext(DbContextOptions<SimpleTasksContext> options)
                    : base(options)
        {
        }

        public DbSet<Task> SimpleTasks { get; set; }
    }
}
