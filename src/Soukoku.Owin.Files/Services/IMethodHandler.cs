using System.Threading.Tasks;

namespace Soukoku.Owin.Files.Services
{
    /// <summary>
    /// Interface for something that handles an http method for webdav.
    /// </summary>
    public interface IMethodHandler
    {
        /// <summary>
        /// Handles the request.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="resource">The resource.</param>
        /// <returns></returns>
        Task<int> HandleAsync(Context context);
    }
}