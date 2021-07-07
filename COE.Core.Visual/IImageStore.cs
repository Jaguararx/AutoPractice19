using System.Drawing;
using System.Threading.Tasks;

namespace COE.Core.Visual
{
    public interface IImageStore
    {
        Task AddBaselineAsync(Image image, string tag);
        Task<Image> GetBaselineAsync(string tag);
    }
}