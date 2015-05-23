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
    public interface IProperty
    {
        /// <summary>
        /// Gets the namespace URI.
        /// </summary>
        /// <value>
        /// The namespace URI.
        /// </value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings")]
        string NamespaceUri { get; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        string Name { get; }

        /// <summary>
        /// Serializes the value into the xml element.
        /// </summary>
        /// <param name="element">The element.</param>
        void SerializeValue(XPathNavigator element);

        /// <summary>
        /// Deserializes the value from the xml element.
        /// </summary>
        /// <param name="element">The element.</param>
        void DeserializeValue(XPathNavigator element);
    }
}
