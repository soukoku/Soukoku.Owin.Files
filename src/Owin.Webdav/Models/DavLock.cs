using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soukoku.Owin.Webdav.Models
{
    /// <summary>
    /// Represents a lock on a resource.
    /// </summary>
    public class DavLock
    {
        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        /// <value>
        /// The token.
        /// </value>
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets the scope.
        /// </summary>
        /// <value>
        /// The scope.
        /// </value>
        public LockScopes Scope { get; set; }

        /// <summary>
        /// Gets or sets the type of the lock.
        /// </summary>
        /// <value>
        /// The type of the lock.
        /// </value>
        public LockType LockType { get; set; }

        public string Owner { get; set; }

        public int Timeout { get; set; }
    }
}
