using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api_todo_lisk.App.Models
{
    public class TokenModel
    {
        [Key]
        public Guid TokenId { get; init; }
        [Required][ForeignKey("UserModel")]
        public Guid TokenUserId { get; init; }
        [Required]
        public string TokenValue { get; init; }
        [Required]
        public DateTime Expiration { get; set; }
        [Required]
        public DateTime CreatedAt { get; init; }

        public TokenModel(Guid userid, string token, DateTime expiration)
        {
            TokenId = Guid.NewGuid();
            TokenUserId = userid;
            TokenValue = token;
            Expiration = expiration;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
