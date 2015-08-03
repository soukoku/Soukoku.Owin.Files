using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soukoku.Owin.Files
{
    /// <summary>
    /// Represents a resource mapped by a url.
    /// </summary>
    public class Resource
    {
        public Resource(Context context, string logicalPath, bool isFolder)
        {
            if (context == null) { throw new ArgumentNullException("context"); }

            Context = context;
            LogicalPath = ReformatLogicalPath(logicalPath);
            IsFolder = isFolder;
        }

        static string ReformatLogicalPath(string logicalPath)
        {
            logicalPath = string.IsNullOrEmpty(logicalPath) ? "/" : logicalPath.Replace(Path.DirectorySeparatorChar, '/').TrimEnd('/');
            if (!logicalPath.StartsWith("/", StringComparison.Ordinal))
            {
                logicalPath = "/" + logicalPath;
            }
            return logicalPath;
        }

        /// <summary>
        /// Generates the full URL on the resource.
        /// </summary>
        /// <param name="absolutePath">if set to <c>true</c> then only generate absolute path urls (/path/to/resource), otherwise generate the full url.</param>
        /// <returns></returns>
        public string GenerateUrl(bool absolutePath)
        {
            var url = Context.Request.PathBase + LogicalPath;

            if (absolutePath)
            {
                if (!url.StartsWith("/", StringComparison.Ordinal)) { url = "/" + url; }
            }
            else
            {
                url = Context.Request.Scheme + Uri.SchemeDelimiter + Context.Request.Host + url;
            }

            if (IsFolder &&
                !url.EndsWith("/", StringComparison.Ordinal))
            {
                url += "/";
            }
            return url;
        }

        /// <summary>
        /// Gets the original owin context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        public Context Context { get; private set; }

        /// <summary>
        /// Gets the logical path from root, with leading root slash '/' but not trailing slash.
        /// </summary>
        /// <value>
        /// The logical path.
        /// </value>
        public string LogicalPath { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this resource is folder.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this resource is folder; otherwise, <c>false</c>.
        /// </value>
        public bool IsFolder { get; private set; }

        /// <summary>
        /// Gets the display name.
        /// </summary>
        /// <value>
        /// The display name.
        /// </value>
        public string DisplayName
        {
            get
            {
                // must include path base part in case logical root is not absolute root
                var tentative = Context.Request.PathBase + LogicalPath;
                return Path.GetFileName(tentative);
            }
        }

        /// <summary>
        /// Gets the resource mime type.
        /// </summary>
        /// <value>
        /// The mime type.
        /// </value>
        public string ContentType
        {
            get
            {
                return IsFolder ? null : Context.GetFilesConfig().MimeTypeProvider.GetMimeType(Path.GetExtension(DisplayName));
            }
        }

        /// <summary>
        /// Gets the resource etag.
        /// </summary>
        /// <value>
        /// The mime type.
        /// </value>
        public string ETag
        {
            get
            {
                return Context.GetFilesConfig().ETagGenerator.Generate(this);
            }
        }

        /// <summary>
        /// Gets the byte size.
        /// </summary>
        /// <value>
        /// The byte size.
        /// </value>
        public long Length { get; set; }

        /// <summary>
        /// Gets the content language.
        /// </summary>
        /// <value>
        /// The content language.
        /// </value>
        public string ContentLanguage { get; set; }

        /// <summary>
        /// Gets the creation date in UTC.
        /// </summary>
        /// <value>
        /// The creation date in UTC.
        /// </value>
        public DateTime CreationDateUtc { get; set; }

        /// <summary>
        /// Gets the modified date in UTC.
        /// </summary>
        /// <value>
        /// The modified date in UTC.
        /// </value>
        public DateTime ModifiedDateUtc { get; set; }
    }
}
