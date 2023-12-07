using System;
namespace Fall2023_Assignment4
{
    public static class Const
    {
        public static class Role
        {
            public const string Admin = "Admin";
            public const string Manager = "Manager";
        }

        public static class Policy
        {
            public const string AdminAndManager = "AdminAndManagerPolicy";
            public const string AdminOrManager = "AdminOrManagerPolicy";
        }
    }
}

