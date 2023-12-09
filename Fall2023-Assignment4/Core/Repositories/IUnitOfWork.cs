using System;
using Fall2023_Assignment4.Repositories;

namespace Fall2023_Assignment4.Core.Repositories
{
	public interface IUnitOfWork
	{
		IUserRepository User { get; }

		IRoleRepository Role { get; }
	}
}

