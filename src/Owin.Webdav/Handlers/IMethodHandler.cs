using System.Threading.Tasks;
using Soukoku.Owin.Webdav.Models;
using Soukoku.Owin.Webdav.Responses;

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
        /// <returns></returns>
        Task<StatusCode> HandleAsync(DavContext context, ResourceResponse resource);
    }
}