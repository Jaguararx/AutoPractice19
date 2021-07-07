using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

using OpenQA.Selenium;

namespace COE.Core.Visual
{
    public class ImageDifferenceProcessor : IImageDifferenceProcessor
    {
        /// <summary>
        /// The cell size of any differences to be drawn on the comparison image
        /// </summary>
        private const int CellSize = 16;

        /// <summary>
        /// Defines a grayscale colour filter to be used during image processing to simplify the comparison process
        /// </summary>
        private static readonly ColorMatrix ColorMatrix =
            new ColorMatrix(new[]
            {
                new[] { .3f, .3f, .3f, 0f, 0f },
                new[] { .59f, .59f, .59f, 0f, 0f },
                new[] { .11f, .11f, .11f, 0f, 0f },
                new[] { 0f, 0f, 0f, 1f, 0f },
                new[] { 0f, 0f, 0f, 0f, 1f }
            });

        /// <summary>
        /// Local output directory of visual comparison results
        /// </summary>
        private static readonly string OutputDirectory = "./ComparisonResults/";

        private static readonly string BaselineDirectory = "Baseline/";

        public int CountDifferingPixels(byte[,] differences) => differences.Cast<byte>().Count(cellValue => cellValue > 0);

        /// <summary>
        /// Gets the visual differences between two images
        /// </summary>
        /// <param name="baseline">The baseline image</param>
        /// <param name="comparison">The comparison image</param>
        /// <returns>An instance of VisualComparisonInfo containing the differences between the two images</returns>
        public VisualComparisonInfo GetDifferences(Image baseline, Image comparison, bool ignoreImageSizeMismatch = false)
        {
            var isImageSizeMismatch = baseline.Size != comparison.Size;
            if (!ignoreImageSizeMismatch && isImageSizeMismatch)
            {
                throw new ImageSizeMismatchException("Visual comparison must be performed with image captures of the same size");
            }

            var differenceMatrix = GetDifferenceMatrix(baseline, comparison);
            var compare = new VisualComparisonInfo(differenceMatrix);

            return compare;
        }

        /// <summary>
        /// Gets a 2 dimensional matrix of differences between the two specified images
        /// </summary>
        /// <param name="img1">The baseline image</param>
        /// <param name="img2">The comparison image</param>
        /// <returns>A 2 dimensional byte array containing a map of image differences</returns>
        public byte[,] GetDifferenceMatrix(Image img1, Image img2)
        {
            var width = img1.Width / CellSize;
            var height = img1.Height / CellSize;
            var differences = new byte[width, height];

            using (var bmp1 = PrepareImageForComparison(img1, width, height))
            {
                using (var bmp2 = PrepareImageForComparison(img2, width, height))
                {
                    for (var y = 0; y < height; y++)
                        for (var x = 0; x < width; x++)
                        {
                            var cellValue1 = bmp1.GetPixel(x, y).R;
                            var cellValue2 = bmp2.GetPixel(x, y).R;
                            var cellDifference = (byte)Math.Abs(cellValue1 - cellValue2);

                            differences[x, y] = cellDifference;
                        }
                }
            }
            return differences;
        }

        /// <summary>
        /// Creates a new image with all the specified differences drawn on top
        /// </summary>
        /// <param name="baseImage">The original image</param>
        /// <param name="differences">A 2 dimensional byte array containing a map of image differences</param>
        /// <returns>The difference image</returns>
        public Bitmap GetDifferenceImage(Image baseImage, byte[,] differences)
        {
            var differenceImage = new Bitmap(baseImage);

            using var brush = new SolidBrush(Color.FromArgb(64, 139, 0, 139));
            using (var g = Graphics.FromImage(differenceImage))
            {
                for (var y = 0; y < differences.GetLength(1); y++)
                    for (var x = 0; x < differences.GetLength(0); x++)
                    {
                        var cellValue = differences[x, y];
                        if (cellValue > 0)
                        {
                            var cellRectangle = new Rectangle(x * CellSize, y * CellSize, CellSize, CellSize);
                            g.DrawRectangle(Pens.DarkMagenta, cellRectangle);
                            g.FillRectangle(brush, cellRectangle);
                        }
                    }
            }
            return differenceImage;
        }

        /// <summary>
        /// Creates a comparison-friendly version of the specified image
        /// </summary>
        /// <param name="original">The original image</param>
        /// <param name="targetWidth">The target width of the new image</param>
        /// <param name="targetHeight">The target height of the new image</param>
        /// <returns>The comparison friendly version of the specified image</returns>
        public Bitmap PrepareImageForComparison(Image original, int targetWidth, int targetHeight)
        {
            var smallVersion = new Bitmap(targetWidth, targetHeight);

            using var g = Graphics.FromImage(smallVersion);
            using var attributes = new ImageAttributes();

            attributes.SetColorMatrix(ColorMatrix);
            attributes.SetWrapMode(WrapMode.TileFlipXY);
            var destRect = new Rectangle(0, 0, targetWidth, targetHeight);
            g.DrawImage(original, destRect, 0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);

            return smallVersion;
        }

        /// <summary>
        /// Draws an ignore region on the specified image
        /// </summary>
        /// <param name="image">The image</param>
        /// <param name="x">The x-coordinate of the upper-left corner of the ignore region</param>
        /// <param name="y">The y-coordinate of the upper-left corner of the ignore region</param>
        /// <param name="width">The width of the ignore region</param>
        /// <param name="height">The height of the ignore region</param>
        /// <returns>The image with ignore region rendered on top</returns>
        public Image DrawIgnoreRegion(Image image, int x, int y, int width, int height)
        {
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            using var brush = new SolidBrush(Color.DarkMagenta);
            using (var g = Graphics.FromImage(image))
            {
                var cellRectangle = new Rectangle(x, y, width, height);
                g.DrawRectangle(Pens.DarkMagenta, cellRectangle);
                g.FillRectangle(brush, cellRectangle);
            }
            return image;
        }

        public Image ScreenshotToImage(Screenshot screenshot)
        {
            using var memStream = new MemoryStream(screenshot.AsByteArray);
            return Image.FromStream(memStream);
        }

        public string SaveLocal(Image image, string fileName, bool isBaseline = false)
        {
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException("Value must be 1 or more characters", nameof(fileName));
            }

            // Remove any invalid characters that we can't perform a save with
            fileName = fileName.Sanitize();
            // Remove any empty spaces and replace with underscore
            fileName = fileName.RemoveSpaces();
            // Remove any existing image format extensions so we can add the correct one
            fileName = Path.ChangeExtension(fileName, ".png");

            string path = isBaseline ? Path.Combine(OutputDirectory, BaselineDirectory) : OutputDirectory;

            Directory.CreateDirectory(path);
            path = Path.Combine(path, fileName);

            image.Save(path, ImageFormat.Png);

            return path;
        }
    }
}
