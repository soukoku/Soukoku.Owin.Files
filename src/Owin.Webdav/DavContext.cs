using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soukoku.Owin.Webdav
{
    /// <summary>
    /// Represents the Owin context associated with the current webdav request.
    /// </summary>
    public sealed class DavContext : Context
    {
        internal DavContext(WebdavConfig config, IDictionary<string, object> environment) : base(environment)
        {
            Config = config;
        }

        /// <summary>
        /// Gets the current <see cref="WebdavConfig"/>.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        public WebdavConfig Config { get; private set; }
    }
}
