using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api_todo_lisk.App.Models
{
    public class TaskModel
    {
        [Key]
        public Guid TaskId { get; init; }

        [Required]
        public string TaskTitle { get; set; }

        [Required]
        public string TaskDescription { get; set; }

        [Required]
        public DateTime TaskDate { get; init; }

        public bool TaskCompleted { get; set; }

        [Required]
        [ForeignKey("UserModel")]
        public Guid TaskUserId { get; init; }

        public TaskModel()
        {
            TaskTitle = string.Empty;
            TaskDescription = string.Empty;
        }

        public TaskModel(Guid userid, string tasktitle, string taskdescription, bool taskcompleted)
        {
            TaskTitle = tasktitle;
            TaskDescription = taskdescription;
            TaskDate = DateTime.UtcNow;
            TaskCompleted = taskcompleted ? taskcompleted : false;
            TaskUserId = userid;
        }

        public void UpdateTask(string title, string description, bool completed)
        {
            TaskTitle = title;
            TaskDescription = description;
            TaskCompleted = completed;
        }
        public void UpdateTaskCheckbox(bool completed)
        {
            TaskCompleted = completed;
        }

    }
}
