using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;

namespace Soukoku.Owin.Webdav.Models
{
    /// <summary>
    /// Represents a property of a webdav resource.
    /// </summary>
    public interface IDavProperty
    {
        /// <summary>
        /// Gets the namespace URI.
        /// </summary>
        /// <value>
        /// The namespace URI.
        /// </value>
        string NamespaceUri { get; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        string Name { get; }

        void SerializeValue(XPathNavigator element);
    }
}
