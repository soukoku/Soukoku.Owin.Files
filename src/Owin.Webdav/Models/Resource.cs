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
    public abstract class Resource : IResource
    {
        protected Resource(IOwinContext context, string logicalPath)
        {
            RequestContext = context;
            LogicalPath = string.IsNullOrEmpty(logicalPath) ? "/" : logicalPath.Replace("\\", "/");

            MakeBuiltInProperties();
        }

        private void MakeBuiltInProperties()
        {
            _properties = new List<IDavProperty>();
            _properties.Add(new CreationDateProperty(this));
            _properties.Add(new DisplayNameProperty(this));
            _properties.Add(new GetContentLanguageProperty(this));
            _properties.Add(new GetContentLengthProperty(this));
            _properties.Add(new GetContentTypeProperty(this));
            _properties.Add(new GetETagProperty(this));
            _properties.Add(new GetLastModifiedProperty(this));
            _properties.Add(new ResourceTypeProperty(this));
        }

        private List<IDavProperty> _properties;
        public IEnumerable<IDavProperty> Properties
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
        public string LogicalPath { get; set; }


        public virtual string DisplayName
        {
            get
            {
                // must be actual url part name even if logical root 
                var tentative = string.Format(CultureInfo.InvariantCulture, "{0}/{1}", RequestContext.Request.PathBase.Value, LogicalPath);
                return Path.GetFileName(tentative);
            }
        }
        public virtual string ContentType { get { return (Type == ResourceType.Resource) ? MimeTypeMap.GetMimeType(Path.GetExtension(DisplayName)) : null; } }
        public virtual string ContentLanguage { get { return null; } }
        public virtual DateTime CreationDateUtc { get { return DateTime.MinValue; } }
        public virtual DateTime ModifiedDateUtc { get { return DateTime.MinValue; } }
        public virtual long Length { get { return 0; } }
        public virtual string ETag { get { return null; } }

        public abstract ResourceType Type { get; }

        public override string ToString()
        {
            return LogicalPath;
        }

        public virtual Stream OpenReadStream()
        {
            throw new NotSupportedException();
        }
    }
}
