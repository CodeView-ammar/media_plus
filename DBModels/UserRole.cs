using System;
using System.Collections.Generic;

namespace MediaPlus.DBModels;

public partial class UserRole
{
    public int RoleId { get; set; }

    public string RoleNameAr { get; set; } = null!;

    public string RoleNameEn { get; set; } = null!;

    public DateTime RoleCdate { get; set; }

    public DateTime? RoleUdate { get; set; }

    public int RoleByuserId { get; set; }

    public string RoleCustCode { get; set; } = null!;

    public int? RoleIsactive { get; set; }
}
