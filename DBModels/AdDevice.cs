using System;
using System.Collections.Generic;

namespace MediaPlus.DBModels;

public partial class AdDevice
{
    public int DevicesId { get; set; }

    public string? DevicesName { get; set; }

    public DateTime? DevicesCdate { get; set; }

    public int? DevicesGroupid { get; set; }

    public DateTime? DevicesUdate { get; set; }

    public string? DevicesCustCode { get; set; }

    public int DeviceIsInterrupt { get; set; }

    public int? DevicesOnoff { get; set; }

    public string? DevicesOfflinePhoto { get; set; }

    public int? DevicesByUserid { get; set; }
}
