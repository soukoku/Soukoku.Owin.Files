using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soukoku.Owin.Files.Services
{
    /// <summary>
    /// Represents a resource mapped by a url.
    /// </summary>
    public interface IResource
    {
        /// <summary>
        /// Gets the original owin context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        Context Context { get; }

        /// <summary>
        /// Gets the logical path from root, with leading root slash '/' but not trailing slash.
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

        /// <summary>
        /// Gets a value indicating whether this resource is folder.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this resource is folder; otherwise, <c>false</c>.
        /// </value>
        bool IsFolder { get; }
    }
}
