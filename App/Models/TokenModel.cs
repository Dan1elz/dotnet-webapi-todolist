using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api_todo_lisk.App.Models
{
    public class TokenModel
    {
        [Key]
        public Guid TokenId { get; init; }

        [Required]
        [ForeignKey("UserModel")]
        public Guid TokenUserId { get; init; }

        [Required]
        public string? TokenValue { get; init; }

        [Required]
        public DateTime TokenCreatedAt { get; init; }

        public TokenModel() {   }

        public TokenModel(Guid userid, string value)
        {
            TokenId = Guid.NewGuid();
            TokenUserId = userid;
            TokenValue = value;
            TokenCreatedAt = DateTime.Now;
        }
    }
}
