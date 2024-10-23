using Microsoft.EntityFrameworkCore;
using ASP.NETCoreWebAPI_Sample.Entitys;

namespace ASP.NETCoreWebAPI_Sample.data
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        // Stelle sicher, dass DbSet<ToDoItem> korrekt definiert ist
        public DbSet<ToDoItem> ToDoItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Optional: Konfiguriere die Tabelle explizit
            modelBuilder.Entity<ToDoItem>().ToTable("ToDoItems");
        }
    }
}
