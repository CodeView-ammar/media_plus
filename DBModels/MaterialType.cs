using System;
using System.Collections.Generic;

namespace MediaPlus.DBModels;

public partial class MaterialType
{
    public int MtypeId { get; set; }

    public string? MtypeNameAr { get; set; }

    public string? MtypeNameEn { get; set; }

    public DateTime? MtypeCdate { get; set; }

    public DateTime? MtypeUdate { get; set; }

    public int? MtypeUserId { get; set; }

    public string? MtypeCustCode { get; set; }

    public int? MtypeIsactive { get; set; }

    public string MtypeStaticHtml { get; set; } = null!;
}
