using Microsoft.Owin;
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
        /// Initializes a new instance of the <see cref="DavResource"/> class.
        /// </summary>
        /// <param name="requestContext">The original request context.</param>
        /// <param name="logicalPath">The logical path.</param>
        protected DavResource(IOwinContext requestContext, string logicalPath)
        {
            RequestContext = requestContext;
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

        public IOwinContext RequestContext { get; private set; }

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
                // must be actual url part name even if logical root 
                var tentative = string.Format(CultureInfo.InvariantCulture, "{0}/{1}", RequestContext.Request.PathBase.Value, LogicalPath);
                return Path.GetFileName(tentative);
            }
        }
        /// <summary>
        /// Gets the resource mime type.
        /// </summary>
        /// <value>
        /// The mime type.
        /// </value>
        public virtual string ContentType { get { return (Type == ResourceType.Resource) ? MimeTypeMap.GetMimeType(Path.GetExtension(DisplayName)) : null; } }

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

        public abstract ResourceType Type { get; }

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
