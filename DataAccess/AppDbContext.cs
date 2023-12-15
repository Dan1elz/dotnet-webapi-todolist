using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.EntityFrameworkCore;
using api_todo_lisk.App.Models;

namespace api_todo_lisk.DataAccess
{
    public class AppDbContext : DbContext
    {
        public DbSet<UserModel> User { get; set; }
        public DbSet<TokenModel> Token { get; set; }
        public DbSet<TaskModel> Task { get; set; }
        public DbSet<CommentModel> Comment { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=api-todo-list.bd");
            optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information); //tirar depois 
            optionsBuilder.EnableSensitiveDataLogging(); // tirar depois

            base.OnConfiguring(optionsBuilder);
        }
    }
}
