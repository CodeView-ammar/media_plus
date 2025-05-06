using System;
using System.Collections.Generic;

namespace MediaPlus.DBModels;

public partial class ShowHtmlcode
{
    public int ShowId { get; set; }

    public string ShowHtmlCode1 { get; set; } = null!;

    public DateTime? ShowCdate { get; set; }

    public DateTime? ShowUdate { get; set; }

    public int? ShowUserid { get; set; }

    public int? ShowIsactive { get; set; }

    public int? ShowByuserId { get; set; }

    public string? ShowCustCode { get; set; }

    public string? ShowCode { get; set; } = null!;

    public int ShowSettingId { get; set; }

}
