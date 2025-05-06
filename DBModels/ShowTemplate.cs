using System;
using System.Collections.Generic;

namespace MediaPlus.DBModels;

public partial class ShowTemplate
{
    public int TempId { get; set; }

    public string? TempNameAr { get; set; }

    public string? TempNameEng { get; set; }

    public int? TempRowNo { get; set; }

    public int? TempColNo { get; set; }

    public string? TempCustCode { get; set; }

    public DateTime? TempCdate { get; set; }

    public DateTime? TempUdate { get; set; }

    public int? TempByUserid { get; set; }

    public int? TempIsactive { get; set; }

    public List<TemplateDetail>? TempDetails { get; set; }
}
