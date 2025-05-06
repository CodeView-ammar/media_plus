using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
 using MediaPlus.DBModels;



namespace MediaPlus.Models.ViewModels
{
    public class ShowCreationViewModel
    {
        public ShowSetting? showSetting{get;set;}
        public List<Show>? show{get;set;}
        public List<ShowDetail>? showDetail{get;set;}
        public List<ShowHtmlcode>? showHTML{get;set;}
        
        public bool  IsScheduled { get; set; }
        public DateTime? ScheduledFrom { get; set; }
        public DateTime? ScheduledTo { get; set; }
        public int? ScheduledRunNo { get; set; }
    
    }
}