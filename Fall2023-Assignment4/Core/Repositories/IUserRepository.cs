using System;
using Fall2023_Assignment4.Data;

namespace Fall2023_Assignment4.Repositories
{
	public interface IUserRepository
	{
		ICollection<ApplicationUser> GetUsers();

        ApplicationUser GetUser(string id);

		ApplicationUser UpdateUser(ApplicationUser user); 
    }
}

