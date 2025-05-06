using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaPlus.Models.ViewModels
{
    public class RolePermissionViewModel
    {
        public int RoleId { get; set; }
        
        public string? RoleName { get; set; }

        public List<UserPermissionViewModel>? Permissions { get; set; }
    }
}