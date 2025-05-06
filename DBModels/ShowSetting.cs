using System;
using System.Collections.Generic;

namespace MediaPlus.DBModels;

public partial class ShowSetting
{
    public int ShowSettingId { get; set; }

    public string? ShowSettingShowcode { get; set; }

    public int? ShowSettingGroupId { get; set; }

    public int? ShowSettingDeviceId { get; set; }

    public int? ShowSettingPresent { get; set; }

    public int? ShowSettingNext { get; set; }

    public int? ShowSettingTotalView { get; set; }

    public DateTime? ShowSettingCdate { get; set; }

    public DateTime? ShowSettingUdate { get; set; }

    public string? ShowSettingCustCode { get; set; } = null!;
}
