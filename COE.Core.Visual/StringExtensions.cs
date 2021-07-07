using System;
using System.IO;
using System.Linq;
using System.Text;

namespace COE.Core.Visual
{
    public static class StringExtensions
    {

        public static string RemoveSpaces(this string fileName) => fileName.Replace(" ", "_");

        /// <summary>
        /// Remove any invalid filename characters 
        /// </summary>
        /// <param name="fileName">The file name</param>
        /// <returns>The file name with invalid characters removed</returns>
        public static string Sanitize(this string fileName)
        {
            var invalidChars = Path.GetInvalidFileNameChars();

            if (fileName.IndexOfAny(invalidChars) != -1)
            {
                var sb = new StringBuilder();
                foreach (var c in fileName)
                {
                    if (!invalidChars.Contains(c))
                    {
                        sb.Append(c);
                    }
                }

                return sb.ToString();
            }

            return fileName;
        }
    }
}