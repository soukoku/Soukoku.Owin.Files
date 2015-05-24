using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Soukoku.Owin.Webdav.Models;
using System.Net;

namespace Soukoku.Owin.Webdav.Models
{
    /// <summary>
    /// Indicates the allowed actions on a <see cref="IResource"/>.
    /// </summary>
    [Flags]
    public enum AllowedActions
    {
        /// <summary>
        /// No actions allowed.
        /// </summary>
        None = 0,
        /// <summary>
        /// Can read properties and file data.
        /// </summary>
        Read = 0x1,
        /// <summary>
        /// Can write file data.
        /// </summary>
        Write = 0x2,
        /// <summary>
        /// Can delete resource.
        /// </summary>
        Delete = 0x4,


        /// <summary>
        /// Can copy resource.
        /// </summary>
        Copy = 0x10,
        /// <summary>
        /// Can move resource.
        /// </summary>
        Move = 0x20,
        /// <summary>
        /// Can create new collection resource.
        /// </summary>
        CreateCollection = 0x40,
        /// <summary>
        /// Can update properties.
        /// </summary>
        UpdateProperty = 0x80,

        /// <summary>
        /// Can lock resource.
        /// </summary>
        Lock = 0x100,

        //Write = 0x200,
        //Write = 0x400,
        //Write = 0x800,
    }

    /// <summary>
    /// Http status codes used by webdav.
    /// </summary>
    //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Not for http codes it ain't.")]
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
    }

    /// <summary>
    /// Indicates the function level of a webdav implementation.
    /// </summary>
    [Flags]
    public enum DavClasses
    {
        /// <summary>
        /// Supports basic webdav behaviors.
        /// </summary>
        Class1 = 1,
        /// <summary>
        /// Supports locking behaviors.
        /// </summary>
        Class2 = 2,
        /// <summary>
        /// Supoorts all revisions to rfc2518 in rfc4918.
        /// </summary>
        Class3 = 4
    }

    public enum LockType
    {
        None = 0,
        Exclusive,
        Shared
    }
}
