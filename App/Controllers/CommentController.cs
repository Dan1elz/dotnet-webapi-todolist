using api_todo_lisk.App.Models;
using api_todo_lisk.DataAccess;
using api_todo_lisk.DataAccess.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api_todo_lisk.App.Controllers
{
    public static class CommentController
    {
        public static void CommentRoutes(this WebApplication app)
        {
            var commentRoutes = app.MapGroup(prefix: "/comment");

            commentRoutes.MapGet("/{taskId}", handler: async (Guid taskId, HttpContext httpContext, AppDbContext context, CancellationToken ct) =>
            {
                try
                {
                    var comments = await context.Comments
                        .Where(u => u.CommentTaskId == taskId)
                        .Select(u => new GetComments(u.CommentId, u.CommentTitle, u.CommentText, u.CommentDateUpdate, u.CommentDate))
                        .ToListAsync(ct);

                    if(comments == null)
                        return Results.Conflict(error: "Comments Não encontrados.");
                
                    return Results.Ok( new { data = comments, message = "Comments encontrados com sucesso."});
                } catch(Exception ex)
                {
                    return Results.BadRequest(new { message = "Erro interno no servidor: " + ex.Message });
                }
            }).RequireAuthorization();

            commentRoutes.MapPost("", handler: async(HttpContent httpContent, CommentsRequestsDto request, AppDbContext context, CancellationToken ct) =>
            {
                try
                {
                    var newComment = new CommentModel(request.CommentText, request.CommentText, request.CommentTaskId);
                    await context.Comments.AddAsync(newComment, ct);
                    await context.SaveChangesAsync(ct);

                    return Results.Ok("Comment criado com sucesso");

                } catch(Exception ex)
                {
                    return Results.BadRequest(new { message = "Erro interno no servidor: " + ex.Message });
                }
            }).RequireAuthorization();

            commentRoutes.MapPut("", handler: async (HttpContext httpContext, CommentsUpdateRequestDto request, AppDbContext context, CancellationToken ct) =>
            {
                try
                {
                    var CommentData = await context.Comments
                        .SingleOrDefaultAsync(u => u.CommentId == request.CommentId, ct);

                    if(CommentData == null)
                        return Results.Conflict(error: "Comment não encontrado.");
                    
                    CommentData.UpdateComment(request.CommentTitle, request.CommentText);
                    await context.SaveChangesAsync(ct);

                    return Results.Ok("Comment atualizado con sucesso.");
                }catch (Exception ex)
                {
                    return Results.BadRequest(new { message = "Erro interno no servidor: " + ex.Message });
                }
            }).RequireAuthorization();

            commentRoutes.MapDelete("/{CommentId}", handler: async(Guid CommentId, HttpContext httpContext, AppDbContext context, CancellationToken ct) =>
            {
                try
                {
                    var CommentData = await context.Comments
                        .SingleOrDefaultAsync(u => u.CommentId == CommentId, ct);
                    
                    if(CommentData == null)
                        return Results.Conflict(error: "Comment não encontrado.");
                    
                    context.Comments.Remove(CommentData);
                    await context.SaveChangesAsync(ct);

                    return Results.Ok("Comment deletado com sucesso.");
                } catch (Exception ex)
                {
                    return Results.BadRequest(new { message = "Erro interno no servidor: " + ex.Message });
                }
            }).RequireAuthorization();

        }
    }
}
