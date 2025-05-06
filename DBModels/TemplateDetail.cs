using System;
using System.Collections.Generic;

namespace MediaPlus.DBModels;

public partial class TemplateDetail
{
    public int TempDetail { get; set; }

    public int? TempTempId { get; set; }

    public double? TempZoneCode { get; set; }

    public int? TempZoneWidth { get; set; }

    public int? TempZoneHeight { get; set; }

    public int? TempZoneFileTypeid { get; set; }

    public string? TempCustCode { get; set; }

    public DateTime? TempCdate { get; set; }

    public DateTime? TempUdate { get; set; }

    public int? TempByUserid { get; set; }

    public int? TempIsactive { get; set; }
}
