namespace COE.Core.Visual
{
    public class ComparisonResult
    {
        public ImageResult Difference { get; set; }
        public ImageResult Baseline { get; set; }
        public float DifferencePercentage { get; set; }
        public bool Match { get; set; }
    }
}