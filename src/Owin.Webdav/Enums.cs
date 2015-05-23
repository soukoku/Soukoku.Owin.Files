using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Soukoku.Owin.Webdav.Models;
using System.Net;

namespace Soukoku.Owin.Webdav
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
    /// Indicates the function level of a webdav implementation.
    /// </summary>
    [Flags]
    public enum DavClasses
    {
        /// <summary>
        /// Basic webdav.
        /// </summary>
        Class1 = 1,
        Class2 = 2,
        Class3 = 4
    }

    /// <summary>
    /// Http status codes used by webdav.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Not for http codes it ain't.")]
    public enum StatusCode
    {
        // spec section 11

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi", Justification = "Useless word block from ancient times.")]
        MultiStatus = 207,
        UnprocessableEntity = 422,
        Locked = 423,
        FailedDependency = 424,
        InsufficientStorage = 507,

        // standard codes
        OK = HttpStatusCode.OK,
        NotFound = HttpStatusCode.NotFound,
        PreconditionFailed = HttpStatusCode.PreconditionFailed,
        Conflict = HttpStatusCode.Conflict,
        NoContent = HttpStatusCode.NoContent,
        Forbidden = HttpStatusCode.Forbidden,
    }
}
