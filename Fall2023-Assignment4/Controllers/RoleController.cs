using Fall2023_Assignment4;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fall2023_Assignment4.Controllers
{
    public class RoleController : Controller
    {
      public IActionResult Index()
        {
            return View();
        }

        [Authorize(Policy = Const.Policies.RequireManager)]
        public IActionResult Manager()
        {
            return View();
        }

        [Authorize(Roles = $"{Const.Roles.Administrator},{Const.Roles.Manager}")]
        public IActionResult Admin()
        {
            return View();
        }
    }
}