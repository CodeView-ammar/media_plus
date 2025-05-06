using System;
using System.Collections.Generic;

namespace MediaPlus.DBModels;

public partial class ShowContent
{
    public int ContentsId { get; set; }

    public string? ContentsShowCode { get; set; }

    public int? ContentsShowId { get; set; }

    public string? ContentsTxt { get; set; }

    public DateTime? ContentsCdate { get; set; }

    public DateTime? ContentsUdate { get; set; }

    public int? ContentsByUserid { get; set; }

    public string? ContentsCustCode { get; set; }

    public int? ContentsIsactive { get; set; }
}
