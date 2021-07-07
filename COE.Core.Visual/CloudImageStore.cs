using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace COE.Core.Visual
{
    public class CloudImageStore : IImageStore
    {
        private static readonly string _baselineImagesContainerName = "baseline-images";
        private readonly BlobContainerClient _baselineImagesContainer;

        public CloudImageStore(BlobServiceClient blobServiceClient)
        {
            if (blobServiceClient == null)
            {
                throw new ArgumentNullException(nameof(blobServiceClient));
            }

            _baselineImagesContainer = blobServiceClient.GetBlobContainerClient(_baselineImagesContainerName);

            if (!_baselineImagesContainer.Exists())
            {
                _baselineImagesContainer.Create();
            }
        }

        /// <summary>
        /// Adds the specified baseline image to the store
        /// </summary>
        /// <param name="image">The baseline image to be stored</param>
        /// <param name="tag">The tag to be associated with the image</param>
        /// <returns>The task</returns>
        public async Task AddBaselineAsync(Image image, string tag)
        {
            var blobClient = _baselineImagesContainer.GetBlobClient(tag);

            using var memStream = new MemoryStream();
            image.Save(memStream, image.RawFormat);

            memStream.Position = 0;

            await blobClient.UploadAsync(memStream, true);

            memStream.Close();
        }

        /// <summary>
        /// Retrieve the baseline image from the store using the specified tag as a key
        /// </summary>
        /// <param name="tag">The tag for the image to be retrieved</param>
        /// <returns>The baseline image</returns>
        public async Task<Image> GetBaselineAsync(string tag)
        {
            var blobClient = _baselineImagesContainer.GetBlobClient(tag);

            // Check if blob exists
            if (!blobClient.Exists())
            {
                return null;
            }

            BlobDownloadInfo imageInfo = await blobClient.DownloadAsync();

            var image = Image.FromStream(imageInfo.Content);

            return image;
        }
    }
}