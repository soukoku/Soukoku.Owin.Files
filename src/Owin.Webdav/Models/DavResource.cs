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
        /// <param name="pathBase">The path base.</param>
        /// <param name="logicalPath">The logical path.</param>
        protected DavResource(string pathBase, string logicalPath)
        {
            PathBase = pathBase ?? string.Empty;
            LogicalPath = string.IsNullOrEmpty(logicalPath) ? "/" : logicalPath.Replace("\\", "/");

            MakeBuiltInProperties();
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
        public IEnumerable<IProperty> Properties
        {
            get { return _properties; }
        }


        //public T FindProperty<T>(string name) where T : class, IDavProperty
        //{
        //    return FindProperty<T>(name, Consts.XmlNamespace);
        //}
        //public T FindProperty<T>(string name, string namespaceUri) where T : class, IDavProperty
        //{
        //    return _properties.FirstOrDefault(p => p.Name == name && p.NamespaceUri == namespaceUri) as T;
        //}

        //public void AddProperties(IEnumerable<IDavProperty> properties)
        //{
        //    if (properties != null)
        //    {
        //        foreach (var p in properties)
        //        {
        //            AddProperty(p);
        //        }
        //    }
        //}
        //public void AddProperty(IDavProperty davProperty)
        //{
        //    if (davProperty != null)
        //    {
        //        // TODO: check dupes?
        //        _properties.Add(davProperty);
        //    }
        //}

        /// <summary>
        /// Gets the path base before the dav root.
        /// </summary>
        /// <value>
        /// The path base.
        /// </value>
        public string PathBase { get; private set; }

        /// <summary>
        /// Gets the logical path.
        /// </summary>
        /// <value>
        /// The logical path.
        /// </value>
        public string LogicalPath { get; set; }


        /// <summary>
        /// Gets the supported webdav class number when queried by client.
        /// </summary>
        /// <value>
        /// The dav class.
        /// </value>
        public virtual DavClasses DavClass { get { return DavClasses.Class1; } }

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
                var tentative = PathBase + LogicalPath;
                return Path.GetFileName(tentative);
            }
        }
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

        /// <summary>
        /// Gets the byte size.
        /// </summary>
        /// <value>
        /// The byte size.
        /// </value>
        public virtual long Length { get { return 0; } }

        public virtual string ETag { get { return null; } }

        public abstract ResourceType ResourceType { get; }
        
        public virtual LockScopes SupportedLock
        {
            get { return LockScopes.Exclusive | LockScopes.Shared; }
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

        /// <summary>
        /// Opens the the resource stream for reading.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.NotSupportedException"></exception>
        public virtual Stream OpenReadStream()
        {
            throw new NotSupportedException();
        }
    }
}
