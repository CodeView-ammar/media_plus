using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MediaPlus.DBModels;

public partial class Show
{
    public int ShowId { get; set; }

    public int? ShowSettingId { get; set; }

    public string? ShowCode { get; set; } = null!;

    public int? ShowTime { get; set; }

    [Required(ErrorMessage = "يجب اختيار قالب العرض")]
    public int? ShowTemplateId { get; set; }

    public int? ShowOrder { get; set; }

    public int? ShowByUserid { get; set; }

    public DateTime? ShowCdate { get; set; }

    public DateTime? ShowUdate { get; set; }

    public string? ShowCustCode { get; set; }

    public int? ShowIsactive { get; set; }

    //// ✅ الحقول الجديدة المضافة:

    public bool? IsScheduled { get; set; }

    public DateTime? ScheduledFrom { get; set; }

    public DateTime? ScheduledTo { get; set; }

    public int? ScheduledRunNo { get; set; }
}
