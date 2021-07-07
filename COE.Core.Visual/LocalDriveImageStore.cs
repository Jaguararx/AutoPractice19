using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace COE.Core.Visual
{
    public class LocalDriveImageStore : IImageStore
    {
        private readonly string _basePath;
        private DirectoryInfo _baseDir;

        public LocalDriveImageStore(string basePath)
        {
            _basePath = basePath ?? throw new ArgumentNullException(nameof(basePath));
            _baseDir = Directory.CreateDirectory(basePath);
        }

        /// <summary>
        /// Adds the specified baseline image to the store
        /// </summary>
        /// <param name="image">The baseline image to be stored</param>
        /// <param name="tag">The tag to be associated with the image</param>
        /// <returns>The task</returns>
        public async Task AddBaselineAsync(Image image, string tag)
        {
            var path = GetPathFromTag(tag);
            if (File.Exists(path)) {
                File.Delete(path);
            }                
            image.Save(path);
        }

        /// <summary>
        /// Retrieve the baseline image from the store using the specified tag as a key
        /// </summary>
        /// <param name="tag">The tag for the image to be retrieved</param>
        /// <returns>The baseline image</returns>
        public async Task<Image> GetBaselineAsync(string tag)
        {
            var path = GetPathFromTag(tag);
            Image image;
            if (File.Exists(path)) 
            {
                image = Image.FromFile(path);
            }
            else
            {
                image = null;
            }
            return image;
        }

        private string GetPathFromTag(string tag) => _basePath + "/" + tag + ".bmp";
    }
}