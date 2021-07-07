using COE.Core;
using COE.Core.Visual;

namespace COE.Example.Tests
{
    public class TestSettings
    {
        public bool VisualTest { get; set; } = false;
        public bool AllureEnabled { get; set; } = false;
        public StorageSettings StorageSettings { get; set; }
        public WebDriverSettings WebDriverSettings { get; set; }
        public VisualSettings VisualSettings { get; set; }
    }
}