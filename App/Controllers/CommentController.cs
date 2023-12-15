using Microsoft.AspNetCore.Mvc;

namespace api_todo_lisk.App.Controllers
{
    public class CommentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
