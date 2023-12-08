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



        [Authorize(Policy = Const.Policies.RequireAdmin)]

        public IActionResult Manager()

        {

            return View();

        }



        //[Authorize(Policy = "RequireAdmin")]

        [Authorize(Roles = $"{Const.Roles.Administrator},{Const.Roles.Manager}")]

        public IActionResult Admin()

        {

            return View();

        }

    }

}