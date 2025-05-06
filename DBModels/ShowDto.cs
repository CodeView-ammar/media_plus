using System;
using System.Collections.Generic;

namespace MediaPlus.DBModels;

public class ShowDto
{
    public int ShowId { get; set; }
    public int? ShowTime { get; set; }
    public int? ShowTemplateId { get; set; }
    public string? ShowCode { get; set; }
    public int? ShowOrder { get; set; }
    public DateTime? ShowCdate { get; set; }
    public string? ShowCustCode { get; set; }
    public int? ShowIsactive { get; set; }
    public bool? IsScheduled { get; set; }
    public DateTime? ScheduledFrom { get; set; }
    public DateTime? ScheduledTo { get; set; }
    public int? ScheduledRunNo { get; set; }
}
