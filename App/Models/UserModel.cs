using System.ComponentModel.DataAnnotations;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace api_todo_lisk.App.Models
{
    public class UserModel
    {
        [Key]
        public Guid Id { get; init; }

        [Required][MaxLength(25)]
        public string Name { get; private set; }

        [Required][MaxLength(25)]
        public string Lastname { get; private set; }

        [Required][MaxLength(50)][EmailAddress]
        public string Email { get; private set; }

        [Required][MaxLength(20)]
        public string Password { get; private set; }
        public DateTime Created { get; init; }
        public DateTime Updated { get; set; }
        public bool Active { get; private set; }

        public UserModel(string name, string lastname, string email, string password)
        {
            Id = Guid.NewGuid();
            Name = name;
            Lastname = lastname;
            Email = email;
            Password = password;
            Created = DateTime.Now;
            Updated = DateTime.Now;
            Active = true;
        }
        public void Update(string name, string lastname)
        {
            Name = name;
            Lastname = lastname;
            Updated = DateTime.Now;
        }
        public void Delete() 
        {
            Active = false;
        }
    }
}
