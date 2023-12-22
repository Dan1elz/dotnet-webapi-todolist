using Microsoft.EntityFrameworkCore;
using api_todo_lisk.DataAccess;
using api_todo_lisk.DataAccess.Dtos;
using api_todo_lisk.App.Models;
using Microsoft.AspNetCore.Mvc;
using api_todo_lisk.App.Services;

namespace api_todo_lisk.App.Controllers
{
    public static class TaskController
    {
        public static void TaskRoutes(this WebApplication app)
        {
            var taskRoutes = app.MapGroup(prefix: "/task");

            taskRoutes.MapGet("", handler: async (HttpContext httpContext, AppDbContext context, CancellationToken ct) =>
            {
                try
                {
                    var token = httpContext.Request.Headers["Authorization"].ToString();

                    if (string.IsNullOrEmpty(token) || !token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                        return Results.Conflict(error: "Token inválido ou ausente.");

                    token = token.Substring("Bearer ".Length).Trim();

                    var verifyToken = await context.Tokens
                        .FirstOrDefaultAsync(u => u.TokenValue == token, ct);

                    if (verifyToken == null)
                        return Results.Conflict(error: "Token não encontrado.");

                    var tasks = await context.Tasks
                    .Where(u => u.TaskUserId == verifyToken.TokenUserId)
                    .Select(u => new GetTasks(u.TaskId, u.TaskTitle, u.TaskDescription, u.TaskDate, u.TaskCompleted))
                    .ToListAsync(ct);

                    if (tasks == null)
                        return Results.Conflict(error: "Tasks Não encontradas.");

                    return Results.Ok(new {data = tasks, message = "Tasks encontradas" });
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { message = "Erro interno no servidor: " + ex.Message });
                }
            }).RequireAuthorization();

            taskRoutes.MapPost("", handler: async (HttpContext httpContext, TasksRequestsDto request, AppDbContext context, CancellationToken ct) =>
            {
                try
                {
                    var token = httpContext.Request.Headers["Authorization"].ToString();

                    if (string.IsNullOrEmpty(token) || !token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                        return Results.Conflict(error: "Token inválido ou ausente.");

                    token = token.Substring("Bearer ".Length).Trim();

                    var verifyToken = await context.Tokens
                        .FirstOrDefaultAsync(u => u.TokenValue == token, ct);

                    if (verifyToken == null)
                        return Results.Conflict(error: "Token não encontrado.");

                    var newTask = new TaskModel(verifyToken.TokenUserId, request.TaskTitle, request.TaskDescription, request.TaskCompleted);
                    await context.Tasks.AddAsync(newTask, ct);
                    await context.SaveChangesAsync(ct);

                    return Results.Ok("Task criada com sucesso");
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { message = "Erro interno no servidor: " + ex.Message });
                }
            }).RequireAuthorization();
            
            taskRoutes.MapPut("", handler: async (HttpContext httpContext, TaskUpdateRequestDto request, AppDbContext context, CancellationToken ct) =>
            {
                try
                {
                   var token = httpContext.Request.Headers["Authorization"].ToString();

                    if (string.IsNullOrEmpty(token) || !token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                        return Results.Conflict(error: "Token inválido ou ausente.");

                    token = token.Substring("Bearer ".Length).Trim();

                    var verifyToken = await context.Tokens
                        .FirstOrDefaultAsync(u => u.TokenValue == token, ct);

                    if (verifyToken == null)
                        return Results.Conflict(error: "Token não encontrado.");

                    var taskData = await context.Tasks
                        .SingleOrDefaultAsync(u => u.TaskId == request.TaskId, ct);

                    if(taskData == null)
                        return Results.Conflict(error: "Task não encontrada.");
                    
                    taskData.UpdateTask(request.TaskTitle, request.TaskDescription, request.TaskCompleted);
                    await context.SaveChangesAsync(ct);

                    return Results.Ok("Task atualizada con sucesso.");
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { message = "Erro interno no servidor: " + ex.Message });
                }
            }).RequireAuthorization();

            taskRoutes.MapDelete("/{taskId:guid}", handler: async (Guid taskId, HttpContext httpContext, AppDbContext context, CancellationToken ct) =>
            {
                try
                {
                   var token = httpContext.Request.Headers["Authorization"].ToString();

                    if (string.IsNullOrEmpty(token) || !token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                        return Results.Conflict(error: "Token inválido ou ausente.");

                    token = token.Substring("Bearer ".Length).Trim();

                    var verifyToken = await context.Tokens
                        .FirstOrDefaultAsync(u => u.TokenValue == token, ct);

                    if (verifyToken == null)
                        return Results.Conflict(error: "Token não encontrado.");

                    var taskData = await context.Tasks
                        .SingleOrDefaultAsync(u => u.TaskId == taskId, ct);

                    if(taskData == null)
                        return Results.Conflict(error: "Task não encontrada.");

                    context.Tasks.Remove(taskData);
                    await context.SaveChangesAsync(ct);
                    
                    return Results.Ok("Task deletada com sucesso.");
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { message = "Erro interno no servidor: " + ex.Message });
                }
            }).RequireAuthorization();
        }
    }
}
