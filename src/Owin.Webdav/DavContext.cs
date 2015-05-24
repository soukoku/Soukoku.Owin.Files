using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

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


        /// <summary>
        /// Gets the depth specified in the header.
        /// </summary>
        /// <value>
        /// The depth.
        /// </value>
        public int Depth
        {
            get
            {
                int depth;
                var values = Request.Headers[DavConsts.Headers.Depth];
                if (int.TryParse(values, out depth))
                {
                    if (depth != 0 && depth != 1)
                    {
                        depth = int.MaxValue;
                    }
                }
                else
                {
                    depth = int.MaxValue;
                }
                return depth;
            }
        }

        /// <summary>
        /// Reads the request body as XML if applicable.
        /// </summary>
        /// <returns></returns>
        public XmlDocument ReadRequestAsXml()
        {
            XmlDocument doc = null;

            if (Request.Headers.ContentType == "application/xml" ||
                Request.Headers.ContentType == "text/xml")
            {
                // todo: should somehow make it async?
                doc = new XmlDocument();
                doc.Load(Request.Body);
            }
            return doc;
        }

    }
}
