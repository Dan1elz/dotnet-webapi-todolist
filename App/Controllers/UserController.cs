using Microsoft.EntityFrameworkCore;
using api_todo_lisk.DataAccess;
using api_todo_lisk.DataAccess.Dtos;
using api_todo_lisk.App.Models;
using api_todo_lisk.App.Services;

namespace api_todo_lisk.App.Controllers
{
    public static class UserController
    {
        public static void UserRoutes(this WebApplication app)
        {
            var userRoutes = app.MapGroup(prefix: "/user");

            userRoutes.MapGet("", handler: async (HttpContext httpContext, AppDbContext context, CancellationToken ct) =>
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

                    var user = await context.Users
                        .Where(u => u.Id == verifyToken.TokenUserId)
                        .Select(u => new UserRequestDto(u.Id, u.Name, u.Lastname, u.Email))
                        .SingleOrDefaultAsync(ct);

                    if (user == null)
                        return Results.Conflict(error: "Usuario não encontrado.");

                    return Results.Ok(new { data = user, message = "Usuario logado com sucesso!" });
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { message = "Erro interno no servidor: " + ex.Message });

                }
            }).RequireAuthorization();

            userRoutes.MapPost("", handler: async (RegisterUserRequestDto request, AppDbContext context, CancellationToken ct) =>
            {
                try
                {
                    var isUserExists = await context.Users
                        .SingleOrDefaultAsync(user => user.Email == request.Email, ct);

                    if (isUserExists != null)
                    {
                        if (isUserExists.Active == true)
                            return Results.Conflict(error: "Usuario já existente");


                        return Results.Conflict(error: "Usuario desativado");
                    }

                    var newUser = new UserModel(request.Name, request.Lastname, request.Email, request.Password);
                    await context.Users.AddAsync(newUser, ct);
                    await context.SaveChangesAsync(ct);

                    return Results.Ok("Usuário cadastrado com sucesso!");
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { message = "Erro interno no servidor: " + ex.Message });

                }
            });

            userRoutes.MapPost("/login", handler: async (LoginUserRequestDto request, AppDbContext context, CancellationToken ct) =>
            {
                try
                {
                    bool isUserExists = await context.Users.AnyAsync(user => user.Email == request.Email, ct);

                    if (isUserExists == false)
                        return Results.Conflict(error: "Usuário não encontrado");

                    var loginUser = await context.Users
                        .SingleOrDefaultAsync(u => u.Email == request.Email && u.Password == request.Password && u.Active == true, ct);
                    if (loginUser == null)
                        return Results.Conflict(error: "Falha ao autentificar. Verifique os dados enviados");

                    var tokenVerify = await context.Tokens
                        .Where(u => u.TokenUserId == loginUser.Id)
                        .FirstOrDefaultAsync(ct);

                    if (tokenVerify != null)
                        return Results.Ok(new { data = tokenVerify.TokenValue, message = "Token Reutilizado" });


                    var token = TokensService.GenerateToken(loginUser.Id);

                    var newToken = new TokenModel(loginUser.Id, token.TokenValue ?? throw new ArgumentNullException(nameof(token.TokenValue)));
                    await context.Tokens.AddAsync(newToken, ct);
                    await context.SaveChangesAsync(ct);

                    return Results.Ok(new { data = token.TokenValue, message = "Token Criado." });

                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { message = "Erro interno no servidor: " + ex.Message });

                }
            });

            userRoutes.MapDelete("", handler: async (HttpContext httpContext, AppDbContext context, CancellationToken ct) =>
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

                    var user = await context.Users.FindAsync(verifyToken.TokenUserId, ct);
                    if (user == null)
                        return Results.Conflict(error: "Usuario não encontrado.");
                    using (var transaction = await context.Database.BeginTransactionAsync(ct))
                    {
                        try
                        {
                            user.Delete();
                            context.Tokens.Remove(verifyToken);
                            await context.SaveChangesAsync(ct);

                            transaction.Commit();
                            return Results.Ok("Usuario desativado com sucesso!");
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return Results.BadRequest(new { message = "Erro ao desativar usuario: " + ex.Message });
                        }
                    }
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { message = "Erro interno no servidor: " + ex.Message });

                }
            }).RequireAuthorization();

            userRoutes.MapPut("", handler: async (HttpContext httpContext, UpdateUserRequestDto request, AppDbContext context, CancellationToken ct) =>
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

                    var isUser = await context.Users
                        .SingleOrDefaultAsync(u => u.Id == verifyToken.TokenUserId && u.Email == request.Email && u.Password == request.Password, ct);

                    if (isUser == null)
                        return Results.Conflict(error: "Os dados não são condizentes.");

                    isUser.Update(request.Name, request.Lastname);
                    context.Tokens.Remove(verifyToken);
                    await context.SaveChangesAsync(ct);

                    var createToken = TokensService.GenerateToken(isUser.Id);

                    var newToken = new TokenModel(isUser.Id, createToken.TokenValue ?? throw new ArgumentNullException(nameof(createToken.TokenValue)));
                    await context.Tokens.AddAsync(newToken, ct);
                    int save = await context.SaveChangesAsync(ct);

                    if (save > 0)
                        return Results.Ok(new { data = newToken.TokenValue, message = "Usuario Atualizado com sucesso." });


                    return Results.BadRequest("Erro ao salvar o token");
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { message = "Erro interno no servidor: " + ex.Message });

                }
            }).RequireAuthorization();
        }
    }
}
