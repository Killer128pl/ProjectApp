using System;

namespace ProjectApp.Desktop.ViewModels
{
    public class PackageListItemViewModel
    {
        public Guid TrackingNumber { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public float Weight { get; set; }
        public string Size { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Payment { get; set; } = string.Empty;
    }
}