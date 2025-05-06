using System;
using System.Collections.Generic;

namespace MediaPlus.DBModels;

public partial class ShowType
{
    public int ShowTypeId { get; set; }

    public string? ShowTypeNameAr { get; set; }

    public string? ShowTypeNameEng { get; set; }

    public string? ShowTypeCustCode { get; set; }

    public int? ShowTypeByUserid { get; set; }

    public DateTime? ShowTypeCdate { get; set; }

    public DateTime? ShowTypeUdate { get; set; }

    public int? ShowTypeIsactive { get; set; }
}
