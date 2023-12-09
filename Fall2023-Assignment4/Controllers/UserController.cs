using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Fall2023_Assignment4.Core.Repositories;
using Fall2023_Assignment4.Core.ViewModels;
using Fall2023_Assignment4.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using static Fall2023_Assignment4.Const;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Fall2023_Assignment4.Controllers
{
    [Authorize(Roles = $"{Const.Roles.Administrator},{Const.Roles.Manager}")]
    public class UserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly SignInManager<ApplicationUser> _signInManager; 

        public UserController(IUnitOfWork unitOfWork, SignInManager<ApplicationUser> signInManager)
        {
            _unitOfWork = unitOfWork;
            _signInManager = signInManager;
        }
        // GET: /<controller>/

        [Authorize(Roles = Const.Roles.Administrator)]
        public IActionResult Index()
        {
            var users = _unitOfWork.User.GetUsers(); 
            return View(users);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var user = _unitOfWork.User.GetUser(id);

            var roles = _unitOfWork.Role.GetRoles();

            var userRoles = await _signInManager.UserManager.GetRolesAsync(user);

            var roleItems = roles.Select(role =>
            new SelectListItem(
                role.Name,
                role.Id,
                userRoles.Any(ur => ur.Contains(role.Name)))).ToList(); 


            var vm = new EditUserViewModel
            {
                User = user,
                Roles = roleItems
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> OnPostAsync(EditUserViewModel data)
        {
            var user = _unitOfWork.User.GetUser(data.User.Id);

            if (user == null)
            {
                return NotFound(); 
            }

            var userRolesInDb = await _signInManager.UserManager.GetRolesAsync(user);

            foreach (var role in data.Roles)
            {
                var assignedInDb = userRolesInDb.FirstOrDefault(ur => ur == role.Text);
                if (role.Selected)
                {
                    if (assignedInDb == null)
                    {
                        //add
                        await _signInManager.UserManager.AddToRoleAsync(user, role.Text);

                    }
                }
                else
                {
                    if (assignedInDb != null)
                    {
                        //Remove Role
                        await _signInManager.UserManager.RemoveFromRoleAsync(user, role.Text);
                    }
                }
            }

            user.FirstName = data.User.FirstName;
            user.LastName = data.User.LastName;
            user.Email = data.User.Email;

            _unitOfWork.User.UpdateUser(user);

            var claims = new List<Claim>
                    {
                        new Claim("amr", "pwd"),
                    };

            var roles = await _signInManager.UserManager.GetRolesAsync(user);

            if (roles.Any())
            {

                //"Manager,User"
                var roleClaim = string.Join(",", roles);
                claims.Add(new Claim("Roles", roleClaim));
                await _signInManager.SignInWithClaimsAsync(user, false, claims);
            }
            return RedirectToAction("Edit", new {id = user.Id});
        }
    }
}

