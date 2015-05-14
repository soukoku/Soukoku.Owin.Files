using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Soukoku.Owin.Webdav.Models
{
    public abstract class Resource : IResource
    {
        public Resource(IOwinContext context, string logicalPath)
        {
            OriginalContext = context;
            LogicalPath = logicalPath.Replace("\\", "/");
            _properties = new List<IProperty>();
            _properties.Add(new NumberProperty(Consts.PropertyName.GetContentLength));
            _properties.Add(new DateProperty(Consts.PropertyName.CreationDate)
            {
                Formatter = (value) => XmlConvert.ToString(value, XmlDateTimeSerializationMode.Utc) // valid rfc 3339?
            });
            _properties.Add(new DateProperty(Consts.PropertyName.GetLastModified)
            {
                FormatString = "r" // RFC1123 
            });
            _properties.Add(new DerivedProperty<string>(Consts.PropertyName.GetContentType)
            {
                DeriveRoutine = () =>
                {
                    return (Type == ResourceType.File) ? MimeTypes.MimeTypeMap.GetMimeType(Path.GetExtension(DisplayName.Value)) : null;
                }
            });
            _properties.Add(new DerivedProperty<string>(Consts.PropertyName.DisplayName)
            {
                DeriveRoutine = () =>
                {
                    return Path.GetFileName(Url.Trim('/')); // must be actual url part name event if root of dav store
                },
                SerializeRoutine = (prop, doc) =>
                {
                    var node = doc.CreateElement(prop.Name, prop.Namespace);
                    node.InnerText = Uri.EscapeUriString(prop.Value);
                    return node;
                }
            });
        }

        private List<IProperty> _properties;
        public IEnumerable<IProperty> Properties
        {
            get { return _properties; }
        }


        public T FindProperty<T>(string name) where T : class, IProperty
        {
            return FindProperty<T>(name, Consts.XmlNamespace);
        }
        public T FindProperty<T>(string name, string nameSpace) where T : class, IProperty
        {
            return _properties.FirstOrDefault(p => p.Name == name && p.Namespace == nameSpace) as T;
        }

        public void AddProperties(IEnumerable<IProperty> properties)
        {
            if (properties != null)
            {
                foreach (var p in properties)
                {
                    AddProperty(p);
                }
            }
        }
        public void AddProperty(IProperty property)
        {
            if (property != null)
            {
                // TODO: check dupes?
                _properties.Add(property);
            }
        }

        public IOwinContext OriginalContext { get; private set; }
        public string LogicalPath { get; set; }

        public DerivedProperty<string> DisplayName { get { return FindProperty<DerivedProperty<string>>(Consts.PropertyName.DisplayName); } }
        public DerivedProperty<string> ContentType { get { return FindProperty<DerivedProperty<string>>(Consts.PropertyName.GetContentType); } }
        public DateProperty CreateDate { get { return FindProperty<DateProperty>(Consts.PropertyName.CreationDate); } }
        public DateProperty ModifyDate { get { return FindProperty<DateProperty>(Consts.PropertyName.GetLastModified); } }
        public NumberProperty Length { get { return FindProperty<NumberProperty>(Consts.PropertyName.GetContentLength); } }

        public abstract ResourceType Type { get; }
        public virtual string Url
        {
            get
            {
                var tentative = string.Format("{0}://{1}{2}/{3}", OriginalContext.Request.Uri.Scheme, OriginalContext.Request.Uri.Authority, OriginalContext.Request.PathBase.Value, LogicalPath);
                if (Type == ResourceType.Folder && !tentative.EndsWith("/"))
                {
                    tentative += "/";
                }
                return tentative;
            }
        }

        public override string ToString()
        {
            return LogicalPath;
        }

        public virtual Stream GetReadStream()
        {
            throw new NotSupportedException();
        }
        public enum ResourceType
        {
            Folder,
            File,
        }
    }
}
