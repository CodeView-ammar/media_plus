using System;
using System.Collections.Generic;

namespace MediaPlus.DBModels;

public partial class Customer
{
    public int CustId { get; set; }

    public string CustNameAr { get; set; } = null!;

    public string CustNameEn { get; set; } = null!;

    public string CustVat { get; set; } = null!;

    public string CustTrNo { get; set; } = null!;

    public string? CustTel { get; set; }

    public string? CustEmail { get; set; }

    public string? CustMobileNo { get; set; }

    public DateTime CustCdate { get; set; }

    public DateTime CustUdate { get; set; }

    public string CustCode { get; set; } = null!;

    public int? CustLicenseCode { get; set; }

    public string CustLogo { get; set; } = null!;

    public int CustIsactive { get; set; }

    //public string CustToken { get; set; } = null!;
}
