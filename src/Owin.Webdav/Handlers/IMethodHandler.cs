using System.Threading.Tasks;
using Soukoku.Owin.Webdav.Models;

namespace Soukoku.Owin.Webdav.Handlers
{
    /// <summary>
    /// Interface for something that handles an http method for webdav.
    /// </summary>
    interface IMethodHandler
    {
        /// <summary>
        /// Handles the request.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="resource">The resource.</param>
        /// <returns>Returns <code>true</code> if handled by the method, <code>false</code> if not.</returns>
        Task<bool> HandleAsync(Context context, IResource resource);
    }
}