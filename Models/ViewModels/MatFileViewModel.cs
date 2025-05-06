using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MediaPlus.Models.CustomAttributes;
using Microsoft.AspNetCore.Http;

namespace MediaPlus.Models.ViewModels
{
    public class MatFileViewModel
    {
        [MaxFileSize(50 * 1024 * 1024)]
        [AllowedExtensions(new string[] { ".jpg", ".png" , ".gif" , ".jpeg" ,".bmp"
                                    , ".mov", ".wmv" , ".flv" , ".avi" ,".webm",".webm",".mkv",".mp4"})]
        public IFormFile? attachedMaterialFile { get; set; }

        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 10)]
        public string? attachedText { get; set; }
        public object File { get; internal set; }
    }
}