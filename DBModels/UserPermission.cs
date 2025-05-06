using System;
using System.Collections.Generic;

namespace MediaPlus.DBModels;

public partial class UserPermission
{
    public int PermId { get; set; }

    public string? PermName { get; set; }

    public string? PermCustCode { get; set; }

    public DateTime? PermUdate { get; set; }

    public DateTime? PermCdate { get; set; }

    public int? PermIsactive { get; set; }

    public int? PermByUserid { get; set; }
}
