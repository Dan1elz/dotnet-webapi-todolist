using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api_todo_lisk.DataAccess;
using api_todo_lisk.DataAccess.Dtos;
using api_todo_lisk.App.Models;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net;

namespace api_todo_lisk.App.Controllers
{
    public class UserController : Controller
    {
        private readonly string _secretKey;
        public UserController() 
        {
            _secretKey = "adasdaklsdjalksdjaieakdlajd";
        }

        public async Task<IActionResult> GetUser(string authorizationHeader, AppDbContext context, CancellationToken ct)
        {
            try
            {
                if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
                    return BadRequest("Token inválido");

                string token = authorizationHeader.Substring("Bearer ".Length);

                var verifyToken = await context.Tokens
               .FirstOrDefaultAsync(u => u.TokenValue == token, ct);

                if (verifyToken == null)
                    return NotFound("Token não encontrado.");

                var user = await context.Users
                    .Where(u => u.Id == verifyToken.TokenUserId)
                    .Select(u => new UserRequestDto(u.Id, u.Name, u.Lastname, u.Email))
                    .SingleOrDefaultAsync(ct);

                if (user == null)
                    return Conflict(error: "Usuario Não Encontrado");

                return Ok(new { data = user, message = "Usuario Logado com sucesso!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno no servidor: " + ex);
            }
        }
        public async Task<IActionResult> RegisterUser(RegisterUserRequestDto request, AppDbContext context, CancellationToken ct)
        {
            try
            { 
                bool isUserExists = await context.Users.AnyAsync(user => user.Email == request.Email, ct);
            
                if (isUserExists == true)
                    return Conflict(error: "O Email já está registrado.");
            
                var newUser = new UserModel(request.Name, request.Lastname, request.Email, request.Password);
                await context.Users.AddAsync(newUser, ct);
                await context.SaveChangesAsync(ct);

                return new ObjectResult(new { Status = 200, Message = "Usuario Cadastrado com sucesso!" }).ToString();

            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno no servidor: " + ex);
            }
        }
        public async Task<IActionResult> LoginUser(LoginUserRequestDto request, AppDbContext context, CancellationToken ct)
        {
            try
            {
                bool isUserExists = await context.Users.AnyAsync(user => user.Email == request.Email, ct);

                if (isUserExists == false)
                { 
                    return BadRequest("Usuário não encontrado");
                }
                
                var loginUser = await context.Users
                    .SingleOrDefaultAsync(u => u.Email == request.Email && u.Password == request.Password, ct);

             

                if (loginUser == null)
                    return Conflict("Falha ao autenticar. Verifique suas credenciais.");

                var existingToken = await context.Tokens
                    .Where(u => u.TokenUserId == loginUser.Id)
                    .FirstOrDefaultAsync(ct);

                if (existingToken != null)
                    return Ok(new { Data = existingToken.TokenValue, Message = "Token reutilizado" });

                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, loginUser.Id.ToString())
                    // Adicione mais claims conforme necessário
                };

                var key = new SymmetricSecurityKey(Convert.FromBase64String(_secretKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: creds
                );

                var newToken = new TokenModel(loginUser.Id, new JwtSecurityTokenHandler().WriteToken(token));
                await context.Tokens.AddAsync(newToken, ct);
                await context.SaveChangesAsync(ct);

                return Ok(new { data = new JwtSecurityTokenHandler().WriteToken(token), message = "Token criado com sucesso!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno no servidor: " + ex);
            }
        }
        public async Task<IActionResult> DeactivateUser(string authorizationHeader, AppDbContext context, CancellationToken ct)
        {
            try
            {
                if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
                    return BadRequest("Token inválido");

                string token = authorizationHeader.Substring("Bearer ".Length);

                var verifyToken = await context.Tokens
               .FirstOrDefaultAsync(u => u.TokenValue == token, ct);

                if (verifyToken == null)
                    return NotFound("Token não encontrado.");

                var user = await context.Users.FindAsync(verifyToken.TokenUserId);
                if (user == null)
                    return NotFound("Usuário não encontrado.");

                using (var transaction = await context.Database.BeginTransactionAsync(ct))
                {
                    try
                    {
                        user.Delete();
                        context.Tokens.Remove(verifyToken);
                        await context.SaveChangesAsync(ct);

                        transaction.Commit();
                        return Ok("Usuário desativado com sucesso!");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return StatusCode(500, $"Erro ao desativar o usuário: " + ex);
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno no servidor: " + ex);
            }
        }
        public async Task<IActionResult> UpdateUser(string authorizationHeader, UpdateUserRequestDto request, AppDbContext context,CancellationToken ct )
        {
            try
            {
                if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
                    return BadRequest("Token inválido");

                string token = authorizationHeader.Substring("Bearer ".Length);

                var verifyToken = await context.Tokens
               .FirstOrDefaultAsync(u => u.TokenValue == token, ct);

                if (verifyToken == null)
                    return NotFound("Token não encontrado.");

                var isUser = await context.Users
                    .SingleOrDefaultAsync(u => u.Id == verifyToken.TokenUserId && u.Email == request.Email && u.Password == request.Password, ct);

                if (isUser == null)
                    return Conflict(error: "Os dados não são condizentes.");

                isUser.Update(request.Name, request.Lastname);

                context.Tokens.Remove(verifyToken);
                await context.SaveChangesAsync(ct);

                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, isUser.Id.ToString())
                };
                    var key = new SymmetricSecurityKey(Convert.FromBase64String(_secretKey));
                    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                    var newToken = new JwtSecurityToken(
                        claims: claims,
                        expires: DateTime.Now.AddDays(1),
                        signingCredentials: creds
                    );

                    var requestToken = new TokenModel(isUser.Id, new JwtSecurityTokenHandler().WriteToken(newToken));
                    await context.Tokens.AddAsync(requestToken, ct);
                    int save = await context.SaveChangesAsync(ct);
                if (save > 0)
                    return Ok(new { data = new JwtSecurityTokenHandler().WriteToken(newToken), message = "Token Criado com sucesso!" });

                return BadRequest("Erroao salvar o token");
            }
            catch(Exception ex)
            {
                return StatusCode(500, "Erro interno no servidor: " + ex);
            }
        }
    }
}
