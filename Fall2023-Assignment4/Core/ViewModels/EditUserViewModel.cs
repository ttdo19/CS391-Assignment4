using System;
using Fall2023_Assignment4.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Fall2023_Assignment4.Core.ViewModels
{
	public class EditUserViewModel
	{
		public ApplicationUser User {get; set; }

		public IList<SelectListItem> Roles { get; set; }
	}
}

