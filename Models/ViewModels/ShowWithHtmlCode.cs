namespace MediaPlus.Models.ViewModels
{
    public class ShowWithHtmlCode
    {
         public int ShowId { get; set; }
            public string? ShowCode { get; set; }
            public int? ShowTime { get; set; }
            public int? ShowOrder { get; set; }
            public int? ShowByUserId { get; set; }
            public DateTime? ShowCDate { get; set; }
            public DateTime? ShowUDate { get; set; }
            public string? ShowCustCode { get; set; }
            public int? ShowIsActive { get; set; }

             public string? ShowHtmlCode { get; set; }



        public bool? IsScheduled { get; set; }

        public DateTime? ScheduledFrom { get; set; }

        public DateTime? ScheduledTo { get; set; }

        public int? ScheduledRunNo { get; set; }

    }
}
