using System;
using System.Collections.Generic;

namespace MediaPlus.DBModels;

public partial class CustomShow
{
    public int CustomId { get; set; }

    public string? CustomDeviceId { get; set; }

    public int? CustomMaterialId { get; set; }

    public string? CustomHtmlCode { get; set; }

    public TimeSpan? CustomFromTime { get; set; }

    public TimeSpan? CustomToTime { get; set; }

    public string? CustomCustCode { get; set; }

    public int? CustomByuserId { get; set; }

    public DateTime? CustomCdate { get; set; }

    public DateTime? CustomUdate { get; set; }

    public int? CustomIsactive { get; set; }
}
