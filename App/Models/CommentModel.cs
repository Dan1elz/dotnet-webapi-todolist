using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace api_todo_lisk.App.Models
{
    public class CommentModel
    {
        [Key]
        public Guid CommentId { get; init; }

        [Required]
        public string CommentText { get; set; }

        [Required]
        public DateTime CommentDate { get; init; }

        [Required]
        [ForeignKey("TaskModel")]
        public Guid CommentTaskId { get; init; }

        public CommentModel()
        {
            CommentText = string.Empty;
        }
        public CommentModel(string commenttext, Guid taskid)
        {
            CommentId = Guid.NewGuid();
            CommentText = commenttext;
            CommentDate = DateTime.UtcNow;
            CommentTaskId = taskid;

        }
    }

}
