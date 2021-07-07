using System;
using System.Drawing;
using System.Threading.Tasks;

namespace COE.Core.Visual
{
    public class VisualResultFactory
    {
        private readonly IImageDifferenceProcessor _imageProcessor;
        private readonly IImageStore _imageStore;

        public VisualResultFactory(IImageDifferenceProcessor imageProcessor, IImageStore imageStore)
        {
            _imageProcessor = imageProcessor ?? throw new ArgumentNullException(nameof(imageProcessor));
            _imageStore = imageStore ?? throw new ArgumentNullException(nameof(imageStore));
        }

        private ComparisonResult CreateBaseResult(VisualComparisonInfo comparisonInfo, Image baselineImage, Image imageFromWebDriver, string tag)
        {
            var diffImage = _imageProcessor.GetDifferenceImage(imageFromWebDriver, comparisonInfo.DifferenceMatrix);
            var baselineImagePath = _imageProcessor.SaveLocal(baselineImage, tag, isBaseline: true);
            var diffImagePath = _imageProcessor.SaveLocal(diffImage, tag, isBaseline: false);

            return new ComparisonResult
            {
                Difference = new ImageResult { Image = diffImage, Path = diffImagePath },
                Baseline = new ImageResult { Image = baselineImage, Path = baselineImagePath },
                DifferencePercentage = comparisonInfo.DifferencePercentage
            };
        }

        public ComparisonResult ImageMatchSuccessResult(Image baselineImage, string tag)
        {
            return new ComparisonResult
            {
                Match = true,
                Baseline = new ImageResult
                {
                    Image = baselineImage,
                    Path = _imageProcessor.SaveLocal(baselineImage, tag, isBaseline: true)
                }
            };
        }

        public ComparisonResult ImageMatchFailureResult(VisualComparisonInfo comparisonInfo, Image baselineImage, Image diffImage, string tag)
        {
            var result = CreateBaseResult(comparisonInfo, baselineImage, diffImage, tag);
            result.Match = false;
            return result;
        }

        public async Task<ComparisonResult> UpdateBaselineResult(VisualComparisonInfo comparisonInfo, Image baselineImage, Image diffImage, string tag)
        {
            var result = CreateBaseResult(comparisonInfo, baselineImage, diffImage, tag);
            await _imageStore.AddBaselineAsync(diffImage, tag).ConfigureAwait(false);
            result.Match = true;
            return result;
        }

        public async Task<ComparisonResult> NoExistingBaselineResult(Image baselineImage, Image diffImage, string tag)
        {
            await _imageStore.AddBaselineAsync(diffImage, tag).ConfigureAwait(false);
            return new ComparisonResult
            {
                Match = true,
                Baseline = new ImageResult
                {
                    Image = diffImage,
                    Path = _imageProcessor.SaveLocal(diffImage, tag, isBaseline: true)
                }
            };
        }
    }
}
