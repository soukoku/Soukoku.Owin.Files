using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soukoku.Owin.Webdav.Models
{
    /// <summary>
    /// Represents a webdav resource.
    /// </summary>
    public interface IResource
    {
        /// <summary>
        /// Gets the logical path.
        /// </summary>
        /// <value>
        /// The logical path.
        /// </value>
        string LogicalPath { get; }

        IEnumerable<IDavProperty> Properties { get; }

        //T FindProperty<T>(string name, string namespaceUri) where T : class, IDavProperty;

        //void AddProperties(IEnumerable<IDavProperty> properties);
        //void AddProperty(IDavProperty davProperty);

        /// <summary>
        /// Opens the the resource stream for reading.
        /// </summary>
        /// <returns></returns>
        Stream OpenReadStream();

        #region built-in webdav properties

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

        ResourceType Type { get; }
        string ETag { get; }

        #endregion
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
