namespace api_todo_lisk.DataAccess.Dtos
{
    public record UserRequestDto(Guid Id, string Name, string Lastname, string Email);


    public record RegisterUserRequestDto(string Name,  string Lastname, string Email, string Password);

    public record LoginUserRequestDto(string Email, string Password);

    public record UpdateUserRequestDto(string Name, string Lastname, string Email, string Password);

}
