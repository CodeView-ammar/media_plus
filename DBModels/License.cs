using System;
using System.Collections.Generic;

namespace MediaPlus.DBModels;

public partial class License
{
    public int LicId { get; set; }

    public DateTime LicStartAt { get; set; }

    public DateTime LicEndAt { get; set; }

    public int LicDeviceNo { get; set; }

    public int LicUserNo { get; set; }

    public DateTime LicCdate { get; set; }

    public DateTime LicUdate { get; set; }

    public string LicCustCode { get; set; } = null!;

    public int LicIsactive { get; set; }

    public int LicMachineNo { get; set; }  // عدد الأجهزة
}
