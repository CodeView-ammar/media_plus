using System;
using System.Collections.Generic;

namespace MediaPlus.DBModels;

public partial class RoleWithPermission
{
    public int RwpId { get; set; }

    public int? RwpRoleId { get; set; }

    public int? RwpPermissionId { get; set; }

    public int? RwpByUserid { get; set; }

    public DateTime? RwpCdate { get; set; }

    public DateTime? RwpUdate { get; set; }

    public string? RwpCustCode { get; set; }
}
