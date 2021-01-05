using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LacedShared.Enums
{
    public enum Permission
    {
        User = 0,
        Moderator = 5,
        Administrator = 10,
        SuperAdministrator = 15,
        Developer = 20,
        Owner = 25
    }
    public static class Permissions
    {
        public static Permission[] OwnerPerms = new Permission[] { Permission.Owner };
        public static Permission[] DevPerms = new Permission[] { Permission.Developer, Permission.Owner };
        public static Permission[] SAdminPerms = new Permission[] { Permission.SuperAdministrator, Permission.Developer, Permission.Owner };
        public static Permission[] AdminPerms = new Permission[] { Permission.Administrator, Permission.SuperAdministrator, Permission.Developer, Permission.Owner };
        public static Permission[] ModPerms = new Permission[] { Permission.Moderator, Permission.Administrator, Permission.SuperAdministrator, Permission.Developer, Permission.Owner };
        public static Permission[] UserPerms = new Permission[] { Permission.User, Permission.Moderator, Permission.Administrator, Permission.SuperAdministrator, Permission.Developer, Permission.Owner };
    }
}
