using System;
using Fall2023_Assignment4.Core.Repositories;
using Fall2023_Assignment4.Data;
using Microsoft.AspNetCore.Identity;

namespace Fall2023_Assignment4.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly ApplicationDbContext _context;

        public RoleRepository(ApplicationDbContext context)
        {
            _context = context; 
        }

        public ICollection<IdentityRole> GetRoles()
        {
            return _context.Roles.ToList(); 
        }
    }
}

