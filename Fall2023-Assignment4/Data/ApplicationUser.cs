using System;

using System.Collections.Generic;

using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Fall2023_Assignment4.Models;
namespace Fall2023_Assignment4.Data;

public class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    public List<Restaurant>? FavoriteRestaurants { get; set; }
}



public class ApplicationRole : IdentityRole
{

}