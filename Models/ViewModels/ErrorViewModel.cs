using System;

namespace MediaPlus.Models.ViewModels
{
    public class ErrorViewModel
    {
        public required string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
