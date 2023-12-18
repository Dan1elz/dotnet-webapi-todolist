using api_todo_lisk.App.Controllers;
using api_todo_lisk.App.Models;
using api_todo_lisk.DataAccess;
using api_todo_lisk.DataAccess.Dtos;
using Microsoft.AspNetCore.Http;
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

            userRoutes.MapGet("", handler: async (HttpContext httpContext, AppDbContext context, CancellationToken ct) =>
            {
                string authorizationHeader = httpContext.Request.Headers["Authorization"];

                if (authorizationHeader != null)
                    await controller.GetUser(authorizationHeader, context, ct);

                else
                { 
                    httpContext.Response.StatusCode = 400; // Bad Request
                   await httpContext.Response.WriteAsync("Cabeçalho de autorização ausente.");
                }
            });

            
            userRoutes.MapPost("", handler: async (RegisterUserRequestDto request, AppDbContext context, CancellationToken ct) =>
            {
                await controller.RegisterUser(request, context, ct);
            });
            
            
            userRoutes.MapPost("/login", handler: async (LoginUserRequestDto request, AppDbContext context, CancellationToken ct) =>
            {
                await controller.LoginUser(request, context, ct);
            });


            userRoutes.MapDelete("", handler: async (HttpContext httpContext, AppDbContext context, CancellationToken ct) => 
            {
                string authorizationHeader = httpContext.Request.Headers["Authorization"];

                if (authorizationHeader != null)
                    await controller.GetUser(authorizationHeader, context, ct);

                else
                {
                    httpContext.Response.StatusCode = 400; // Bad Request
                    await httpContext.Response.WriteAsync("Cabeçalho de autorização ausente.");
                }
            });

            userRoutes.MapPut("", handler: async (HttpContext httpContext, UpdateUserRequestDto request, AppDbContext context, CancellationToken ct) =>
            {
                string authorizationHeader = httpContext.Request.Headers["Authorization"];

                if (authorizationHeader != null)
                    await controller.GetUser(authorizationHeader, context, ct);

                else
                {
                    httpContext.Response.StatusCode = 400; // Bad Request
                    await httpContext.Response.WriteAsync("Cabeçalho de autorização ausente.");
                }
            });
        }
    }
}
