using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api_todo_lisk.App.Models
{
    public class TaskModel
    {
        [Key]
        public Guid TaskId { get; init; }

        [Required]
        public string? TaskTitle { get; set; }

        [Required]
        public string? TaskDescription { get; set; }

        [Required]
        public DateTime TaskDate { get; init; }

        public bool TaskCompleted { get; set; }

        [Required]
        [ForeignKey("UserModel")]
        public Guid TaskUserId { get; init; }

        public TaskModel()
        {

        }

        public TaskModel(Guid userid, string tasktitle, string taskdescription, bool taskcompleted)
        {
            TaskTitle = tasktitle;
            TaskDescription = taskdescription;
            TaskDate = DateTime.UtcNow;
            TaskCompleted = taskcompleted ? taskcompleted: false;
            TaskUserId = userid;
        }

    }
}
