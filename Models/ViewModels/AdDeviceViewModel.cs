using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MediaPlus.Models.CustomAttributes;

namespace MediaPlus.Models.ViewModels
{
    public class AdDeviceViewModel
    {
        public int DevicesId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string? DevicesName { get; set; }

        public DateTime? DevicesCdate { get; set; }
        public DateTime? DevicesUdate { get; set; }

        [Required(ErrorMessage = "Group is required")]
        public int DevicesGroupid { get; set; }
        public string? DevicesGroupname { get; set; }

        public string? DevicesCustCode { get; set; }
        public string? DevicesCustomerName { get; set; }

        public bool DevicesOnoff { get; set; }

        public string? DevicesOfflinePhoto { get; set; }


        [MaxFileSize(20 * 1024 * 1024)]
        [AllowedExtensions(new string[] { ".jpg", ".png",".jpeg"})]
        public IFormFile? DevicesOfflinePhotoFile { get; set; }

        public int DevicesByUserid { get; set; }
        public string? DevicesByUserName { get; set; }
    }
}