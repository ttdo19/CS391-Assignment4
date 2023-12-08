using System;
namespace Fall2023_Assignment4
{
    public static class Const
    {
        public static class Roles
        {
            public const string Administrator = "Administrator";
            public const string Manager = "Manager";
            public const string User = "User";
        }

        public static class Policies
        {
            public const string RequireManager = "RequireManager";
            public const string RequireAdmin = "RequireAdmin";
        }
    }
}

