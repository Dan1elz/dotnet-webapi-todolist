using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace api_todo_lisk.DataAccess.Dtos
{
    public record UserRequestDto(Guid Id, string Name, string Lastname, string Email);

    public record RegisterUserRequestDto(string Name, string Lastname, string Email, string Password);

    public record LoginUserRequestDto(string Email, string Password);

    public record UpdateUserRequestDto(string Name, string Lastname, string Email, string Password);

    public record GetTasks(Guid TaskId, string TaskTitle, string TaskDescription, DateTime TaskDate, bool TaskCompleted);

    public record TasksRequestsDto(string TaskTitle, string TaskDescription, bool TaskCompleted);

    public record TaskUpdateRequestDto(Guid TaskId, string TaskTitle, string TaskDescription, bool TaskCompleted);

    public record TasksUpdateCheckboxDto(Guid TaskId, bool TaskCompleted);

    public record TaskDeleteRequestDto(Guid TaskId);
    public record GetComments(
        Guid CommentId,
        string CommentText,
        DateTime CommentDate
    );
    public record CommentsRequestsDto(string CommentText, Guid CommentTaskId);

    public record CommentsUpdateRequestDto(Guid CommentId, string CommentText);


}
