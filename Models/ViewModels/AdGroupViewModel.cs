using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MediaPlus.Models.ViewModels
{
    public class AdGroupViewModel
    {
        public int GroupId { get; set; }

        public string? GroupName { get; set; }

        public DateTime? GroupCdate { get; set; }

        public DateTime? GroupUdate { get; set; }

        public bool GroupIsactive { get; set; }

        public string? GroupCustCode { get; set; }
        public string? GroupCustomerName {get; set;}

        public int? GroupByUserid { get; set; }
        public string? GroupUserName {get; set;}
    }
}