using Microsoft.AspNetCore.Mvc;

namespace api_todo_lisk.App.Controllers
{
    public class TaskController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
