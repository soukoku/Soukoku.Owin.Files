using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Soukoku.Owin.Webdav.Responses
{
    /// <summary>
    /// Http status codes used by webdav.
    /// </summary>
    public enum StatusCode
    {
        /// <summary>
        /// Special indicator (not a real http code) for request not handled by webdav.
        /// </summary>
        NotHandled = 0,

        // spec section 11

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi", Justification = "Useless word block from ancient times.")]
        MultiStatus = 207,
        UnprocessableEntity = 422,
        Locked = 423,
        FailedDependency = 424,
        InsufficientStorage = 507,

        // standard codes
        OK = HttpStatusCode.OK,
        Created = HttpStatusCode.Created,
        NoContent = HttpStatusCode.NoContent,
        BadRequest = HttpStatusCode.BadRequest,
        NotFound = HttpStatusCode.NotFound,
        PreconditionFailed = HttpStatusCode.PreconditionFailed,
        Conflict = HttpStatusCode.Conflict,
        UnsupportedMediaType = HttpStatusCode.UnsupportedMediaType,
        Forbidden = HttpStatusCode.Forbidden,
        MethodNotAllowed = HttpStatusCode.MethodNotAllowed
    }
}
