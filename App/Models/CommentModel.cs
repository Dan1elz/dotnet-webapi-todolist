using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace api_todo_lisk.App.Models
{
    public class CommentModel
    {
        [Key]
        public Guid CommentId { get; init; }

        [Required]
        public string CommentTitle { get; set; }

        [Required]
        public string CommentText { get; set; }

        [Required]
        public DateTime CommentDateUpdate { get; set; }

        [Required]
        public DateTime CommentDate { get; init; }

        [Required]
        [ForeignKey("TaskModel")]
        public Guid CommentTaskId { get; init; }

        public CommentModel()
        { 
            CommentTitle = string.Empty;
            CommentText = string.Empty;
        }
        public CommentModel(string commenttitle, string commenttext, Guid taskid)
        {
            CommentId = Guid.NewGuid();
            CommentTitle = commenttitle;
            CommentText = commenttext;
            CommentDate = DateTime.UtcNow;
            CommentDateUpdate = DateTime.UtcNow;
            CommentTaskId = taskid;
          
        }

        public void UpdateComment(string title, string text)
        {
            CommentTitle = title;
            CommentText = text;
            CommentDateUpdate = DateTime.UtcNow;
        }
    }
    
}
