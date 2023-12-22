using api_todo_lisk.App.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using api_todo_lisk;

namespace api_todo_lisk.App.Services
{
    public class TokensService
    {
        public static TokenModel GenerateToken(Guid id)
        {
            var key = Encoding.ASCII.GetBytes(Key.secret);
            var tokenConfig = new SecurityTokenDescriptor
            { 
                Subject = new System.Security.Claims.ClaimsIdentity(new Claim[]
                {
                    new Claim("Id", id.ToString()),
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenConfig);
            var tokenString = tokenHandler.WriteToken(token);

            return new TokenModel(id, tokenString);

        }
    }
}
