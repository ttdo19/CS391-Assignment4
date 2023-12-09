using System;
using Fall2023_Assignment4.Core.Repositories;

namespace Fall2023_Assignment4.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        public IUserRepository User { get; }

        public IRoleRepository Role { get; }

        public UnitOfWork(IUserRepository user, IRoleRepository role)
		{
            User = user;
            Role = role;
		}
    }
}

