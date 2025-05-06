using System;
using System.Collections.Generic;

namespace MediaPlus.DBModels;

public partial class ShowMaterial
{
    public int MatId { get; set; }

    public string MatShowNameAr { get; set; } = null!;

    public string MatShowNameEn { get; set; } = null!;

    public string MatPath { get; set; } = null!;

    public int MatTypeId { get; set; }

    public DateTime MatCdate { get; set; }

    public DateTime? MatUdate { get; set; }

    public int MatByuserId { get; set; }

    public string MatCustCode { get; set; } = null!;

    public int? MatIsactive { get; set; }
}
