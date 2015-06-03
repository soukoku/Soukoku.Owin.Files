using Soukoku.Owin.Webdav.Models.BuiltIn;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Soukoku.Owin.Webdav.Responses;

namespace Soukoku.Owin.Webdav.Models
{
    /// <summary>
    /// A base implementation of <see cref="IResource"/>.
    /// </summary>
    public abstract class DavResource : IResource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DavResource" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="logicalPath">The logical path.</param>
        protected DavResource(DavContext context, string logicalPath)
        {
            Context = context;

            LogicalPath = ReformatLogicalPath(logicalPath);

            MakeBuiltInProperties();
        }

        private string ReformatLogicalPath(string logicalPath)
        {
            logicalPath = string.IsNullOrEmpty(logicalPath) ? "/" : logicalPath.Replace(Path.DirectorySeparatorChar, '/').TrimEnd('/');
            if (!logicalPath.StartsWith("/", StringComparison.Ordinal))
            {
                logicalPath = "/" + logicalPath;
            }
            return logicalPath;
        }

        private void MakeBuiltInProperties()
        {
            _properties = new List<IProperty>();
            _properties.Add(new CreationDateProperty(this));
            _properties.Add(new DisplayNameProperty(this));
            _properties.Add(new GetContentLanguageProperty(this));
            _properties.Add(new GetContentLengthProperty(this));
            _properties.Add(new GetContentTypeProperty(this));
            _properties.Add(new GetETagProperty(this));
            _properties.Add(new GetLastModifiedProperty(this));
            _properties.Add(new ResourceTypeProperty(this));
            _properties.Add(new SupportedLockProperty(this));
        }

        private List<IProperty> _properties;
        protected IEnumerable<IProperty> BuiltInProperties
        {
            get { return _properties; }
        }

        /// <summary>
        /// Gets the current webdav context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        public DavContext Context { get; private set; }

        /// <summary>
        /// Gets the logical path.
        /// </summary>
        /// <value>
        /// The logical path.
        /// </value>
        public string LogicalPath { get; set; }


        /// <summary>
        /// Gets the display name.
        /// </summary>
        /// <value>
        /// The display name.
        /// </value>
        public virtual string DisplayName
        {
            get
            {
                // must include path base part in case logical root is not absolute root
                var tentative = Context.Request.PathBase + LogicalPath;
                return Path.GetFileName(tentative);
            }
        }

        /// <summary>
        /// Gets the byte size.
        /// </summary>
        /// <value>
        /// The byte size.
        /// </value>
        public virtual long Length { get { return 0; } }

        /// <summary>
        /// Gets the resource mime type.
        /// </summary>
        /// <value>
        /// The mime type.
        /// </value>
        public virtual string ContentType { get { return (ResourceType == ResourceType.Resource) ? MimeTypeMap.GetMimeType(Path.GetExtension(DisplayName)) : null; } }

        /// <summary>
        /// Gets the content language.
        /// </summary>
        /// <value>
        /// The content language.
        /// </value>
        public virtual string ContentLanguage { get { return null; } }

        /// <summary>
        /// Gets the creation date in UTC.
        /// </summary>
        /// <value>
        /// The creation date in UTC.
        /// </value>
        public virtual DateTime CreationDateUtc { get { return DateTime.MinValue; } }

        /// <summary>
        /// Gets the modified date in UTC.
        /// </summary>
        /// <value>
        /// The modified date in UTC.
        /// </value>
        public virtual DateTime ModifiedDateUtc { get { return DateTime.MinValue; } }


        public abstract ResourceType ResourceType { get; }

        public virtual string ETag { get { return null; } }

        public virtual LockScopes SupportedLocks
        {
            get { return LockScopes.Exclusive | LockScopes.Shared; }
        }


        public virtual IEnumerable<IProperty> GetProperties(bool nameOnly, IEnumerable<PropertyFilter> filter)
        {
            if (filter.Count() == 0)
            {
                return BuiltInProperties;
            }
            return BuiltInProperties.Where(p => filter.Any(f => f.Name == p.Name && f.XmlNamespace == p.XmlNamespace));
        }

        public virtual IEnumerable<PropertyResponse> SetProperties(IEnumerable<IProperty> setValues, IEnumerable<IProperty> deleteValues)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Generates the full URL on the resource.
        /// </summary>
        /// <param name="pathAbsolute">if set to <c>true</c> then only generate path absolute urls (/path/to/resource), otherwise generate the full url.</param>
        /// <returns></returns>
        public virtual string GenerateUrl(bool pathAbsolute)
        {
            var url = Context.Request.PathBase + LogicalPath;

            if (pathAbsolute)
            {
                if (!url.StartsWith("/", StringComparison.Ordinal)) { url = "/" + url; }
            }
            else
            {
                url = Context.Request.Scheme + Uri.SchemeDelimiter + Context.Request.Host + url;
            }

            if (ResourceType == ResourceType.Collection &&
                !url.EndsWith("/", StringComparison.Ordinal))
            {
                url += "/";
            }
            return url;
        }


        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return LogicalPath;
        }
    }
}
