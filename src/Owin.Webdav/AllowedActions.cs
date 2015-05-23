using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Soukoku.Owin.Webdav.Models;

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
}
