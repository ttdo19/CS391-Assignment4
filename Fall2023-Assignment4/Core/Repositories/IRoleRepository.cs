using System;
using Fall2023_Assignment4.Data;
using Microsoft.AspNetCore.Identity;

namespace Fall2023_Assignment4.Core.Repositories
{
	public interface IRoleRepository
	{
		ICollection<IdentityRole> GetRoles(); 
	}
}

