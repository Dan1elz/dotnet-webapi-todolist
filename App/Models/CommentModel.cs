using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace api_todo_lisk.App.Models
{
    public class CommentModel
    {
        [Key]
        public Guid CommentId { get; init; }

        [Required]
        public string? CommentText { get; init; }

        [Required]
        public DateTime CommentDate { get; init; }

        [Required]
        [ForeignKey("TaskModel")]
        public Guid CommentTaskId { get; init; }

        [Required]
        [ForeignKey("UserModel")]
        public Guid CommentUserId { get; init; }

        public CommentModel()
        { }
        public CommentModel(string commenttext, Guid taskid, Guid userid)
        {
            CommentId = Guid.NewGuid();
            CommentText = commenttext;
            CommentDate = DateTime.UtcNow;
            CommentTaskId = taskid;
            CommentUserId = userid;
        }
    }
    
}
