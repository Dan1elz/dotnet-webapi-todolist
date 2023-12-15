using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api_todo_lisk.App.Models
{
    public class TokenModel
    {
        [Key]
        public Guid TaskId { get; init; }

        [Required]
        [ForeignKey("UserModel")]
        public Guid UserId { get; init; }

        [Required]
        public string? TaskTitle { get; init; }

        [Required]
        public string? TaskDescription { get; init; }

        [Required]
        public bool TaskCompleted { get; init; }

        public TokenModel()
        {
            // Construtor sem parâmetros
        }

        public TokenModel(Guid userid, string tasktitle, string taskdescription, bool taskcompleted)
        {
            TaskId = Guid.NewGuid();
            UserId = userid;
            TaskTitle = tasktitle;
            TaskDescription = taskdescription;
            TaskCompleted = taskcompleted;
        }
    }
}
