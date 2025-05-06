using System;
using System.Collections.Generic;

namespace MediaPlus.DBModels;


public partial class AdGroup
{
    public int GroupId { get; set; }

    public string? GroupName { get; set; }

    public DateTime? GroupCdate { get; set; }

    public DateTime? GroupUdate { get; set; }

    public int? GroupIsactive { get; set; }

    public string? GroupCustCode { get; set; }

    public int? GroupByUserid { get; set; }
}
