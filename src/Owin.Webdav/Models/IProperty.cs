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
        /// Gets the XML namespace.
        /// </summary>
        /// <value>
        /// The XML namespace.
        /// </value>
        string XmlNamespace { get; }

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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode", Justification = "Totally necessary for ease of use.")]
        void SerializeValue(XmlElement element, NewElementFunc newElementMethod);

        /// <summary>
        /// Deserializes the value from the xml element.
        /// </summary>
        /// <param name="element">The element.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode", Justification = "Totally necessary for ease of use.")]
        void DeserializeValue(XmlElement element);
    }

    /// <summary>
    /// A delegate for creating new <see cref="XmlElement"/>.
    /// </summary>
    /// <param name="elementName">Name of the element.</param>
    /// <param name="xmlNamespace">The XML namespace.</param>
    /// <returns></returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode", Justification = "Totally necessary for ease of use.")]
    public delegate XmlElement NewElementFunc(string elementName, string xmlNamespace);
}
