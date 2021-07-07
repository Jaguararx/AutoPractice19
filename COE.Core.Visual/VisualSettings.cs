using System;

namespace COE.Core.Visual
{
    public class VisualSettings
    {
        public bool UseLocalStorage{ get; set; }
        public string LocalStoragePath { get; set; }
        public string ImageStorageConnectionString { get; set; }
        public bool UpdateBaseline { get; set; }
        public TimeSpan WaitBeforeImageComparison { get; set; } = TimeSpan.FromSeconds(5);
    }
}