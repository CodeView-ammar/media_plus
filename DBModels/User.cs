using System;
using System.Collections.Generic;

namespace MediaPlus.DBModels;

public partial class User
{
    public int UserId { get; set; }

    public string UserNameAr { get; set; } = null!;

    public string UserNameEn { get; set; } = null!;

    public string UserLoginName { get; set; } = null!;

    public string UserPassword { get; set; } = null!;

    public string? UserPhoto { get; set; }

    public DateTime UserCdate { get; set; }

    public DateTime? UserUdate { get; set; }

    public int UserRoleId { get; set; }

    public string UserCustCode { get; set; } = null!;
}
