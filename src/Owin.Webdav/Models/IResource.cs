using Soukoku.Owin.Webdav.Responses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soukoku.Owin.Webdav.Models
{
    /// <summary>
    /// Represents a webdav resource mapped by a url.
    /// </summary>
    public interface IResource
    {
        /// <summary>
        /// Gets the current webdav context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        DavContext Context { get; }

        /// <summary>
        /// Gets the logical path from dav root, with leading root slash '/' but not trailing slash.
        /// </summary>
        /// <value>
        /// The logical path.
        /// </value>
        string LogicalPath { get; }

        /// <summary>
        /// Gets the display name.
        /// </summary>
        /// <value>
        /// The display name.
        /// </value>
        string DisplayName { get; }

        /// <summary>
        /// Gets the byte size.
        /// </summary>
        /// <value>
        /// The byte size.
        /// </value>
        long Length { get; }

        /// <summary>
        /// Gets the resource mime type.
        /// </summary>
        /// <value>
        /// The mime type.
        /// </value>
        string ContentType { get; }

        /// <summary>
        /// Gets the content language.
        /// </summary>
        /// <value>
        /// The content language.
        /// </value>
        string ContentLanguage { get; }

        /// <summary>
        /// Gets the creation date in UTC.
        /// </summary>
        /// <value>
        /// The creation date in UTC.
        /// </value>
        DateTime CreationDateUtc { get; }

        /// <summary>
        /// Gets the modified date in UTC.
        /// </summary>
        /// <value>
        /// The modified date in UTC.
        /// </value>
        DateTime ModifiedDateUtc { get; }

        ResourceType ResourceType { get; }

        string ETag { get; }

        LockScopes SupportedLocks { get; }

        /// <summary>
        /// Gets the custom properties associated with the resource.
        /// </summary>
        /// <param name="nameOnly">if set to <c>true</c> then don't retrieve the property value if possible.</param>
        /// <param name="filter">The optional filter. If empty then all properties should be returned.</param>
        /// <returns></returns>
        IEnumerable<IProperty> GetProperties(bool nameOnly, IEnumerable<PropertyFilter> filter);

        /// <summary>
        /// Sets the properties associated with the resource.
        /// </summary>
        /// <param name="setValues">The property values to write.</param>
        /// <param name="deleteValues">The property values to delete.</param>
        /// <returns></returns>
        IEnumerable<PropertyResponse> SetProperties(IEnumerable<IProperty> setValues, IEnumerable<IProperty> deleteValues);
    }
    
    public class PropertyFilter 
    {
        public string XmlNamespace { get; set; }
        public string Name { get; set; }
    }

    /// <summary>
    /// Indicates the webdav resource type.
    /// </summary>
    public enum ResourceType
    {
        /// <summary>
        /// A simple resource.
        /// </summary>
        Resource,
        /// <summary>
        /// A resource collection.
        /// </summary>
        Collection,
    }
}
