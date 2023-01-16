using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senjyouhara.Common.Models
{
    public class UserModel
    {
        public string Token { get; set; }
        public User User { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public List<string> Permissions { get; set; } = new List<string>();
        public List<MenuModel> Routes { get; set; } = new List<MenuModel>();

    }

    public class Permission
    {
        public int MenuId { get; set; }
        public string MenuName { get; set; }
        public string ParentName { get; set; }
        public int ParentId { get; set; }
        public string OrderNum { get; set; }
        public string Component { get; set; }
        public string Query { get; set; }
        public int IsFrame { get; set; }
        public int IsCache { get; set; }
        public string MenuType { get; set; }
        public int Visible { get; set; }
        public string Status { get; set; }
        public string Perms { get; set; }
        public string Icon { get; set; }
        public List<Permission> Children { get; set; } = new List<Permission>();

    }

    public class Role
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string RoleKey { get; set; }
        public string RoleSort { get; set; }
        public string DataScope { get; set; }
        public bool MenuCheckStrictly { get; set; }
        public bool DeptCheckStrictly { get; set; }
        public string Status { get; set; }
        public string DelFlag { get; set; }
        public List<int> MenuIds { get; set; }

    }

    public class User
    {
        public int UserId { get; set; }

        public string UserName { get; set; }

        public string NickName { get; set; }

        public string Email { get; set; }

        public string Phonenumber { get; set; }

        public string Sex { get; set; }

        public string Avatar { get; set; }
        public string Password { get; set; }

        public string Status { get; set; }
        public string CreateTime { get; set; }
        public string LoginDate { get; set; }
        public List<Role> Roles { get; set; }
    }
}
