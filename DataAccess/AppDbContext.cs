﻿using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.EntityFrameworkCore;
using api_todo_lisk.App.Models;

namespace api_todo_lisk.DataAccess
{
    public class AppDbContext : DbContext
    {
        public DbSet<UserModel> Users { get; set; }
        public DbSet<TokenModel> Tokens { get; set; }
        public DbSet<TaskModel> Tasks { get; set; }
        public DbSet<CommentModel> Comments { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string dbPath = Path.Combine(Directory.GetCurrentDirectory(), "DataAccess/Database", "api-todo-list.db");

            optionsBuilder.UseSqlite($"Data Source={dbPath}");

            optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information); //tirar depois 
            optionsBuilder.EnableSensitiveDataLogging(); // tirar depois

            base.OnConfiguring(optionsBuilder);
        }
    }
}
