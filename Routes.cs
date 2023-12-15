using api_todo_lisk.App.Controllers;
using api_todo_lisk.App.Models;
using api_todo_lisk.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api_todo_lisk
{
    public static class Routes
    {
        public static void MapRoutes(this WebApplication app)
        {
            var controller = new UserController();

            var userRoutes = app.MapGroup(prefix: "/user");

            userRoutes.MapGet("/${id}", handler: async (Guid id, AppDbContext context, CancellationToken ct) =>
            {
                await controller.GetUser(id, context, ct);
            });
        }
    }
}
