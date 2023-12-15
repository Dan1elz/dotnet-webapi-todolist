using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api_todo_lisk.DataAccess;

namespace api_todo_lisk.App.Controllers
{
    public class UserController : Controller
    {
        public async Task GetUser(Guid id, DbContext context, CancellationToken ct)
        {
           var user = await context.Users.Where(user => user.Id == id).ToListAsync(ct);
        }
    }
}
