using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MediaPlus.Models.CustomAttributes;
using MediaPlus.Models.CustomAttributes.ValidateDuplications;
using Microsoft.AspNetCore.Mvc;

namespace MediaPlus.Models.ViewModels
{

    public class MediaApiModel
    {
        public string MatShowNameAr { get; set; }
        public string MatShowNameEn { get; set; }
        public int MatTypeId { get; set; }
        public string MatCustCode { get; set; }

        // Â–« ÂÊ «·„·› «·ÕﬁÌﬁÌ (Ê·Ì” «·„”«— ›ﬁÿ)
        public IFormFile MatFile
        {
            get; set;
        }
    }
}