using System.Drawing;

using OpenQA.Selenium;

namespace COE.Core.Visual
{
    public interface IImageDifferenceProcessor
    {
        Image DrawIgnoreRegion(Image image, int x, int y, int width, int height);
        Bitmap GetDifferenceImage(Image baseImage, byte[,] differences);
        byte[,] GetDifferenceMatrix(Image img1, Image img2);
        VisualComparisonInfo GetDifferences(Image baseline, Image comparison, bool ignoreImageSizeMisatch = false);
        Bitmap PrepareImageForComparison(Image original, int targetWidth, int targetHeight);
        string SaveLocal(Image image, string fileName, bool isBaseline = false);
        Image ScreenshotToImage(Screenshot screenshot);
    }
}