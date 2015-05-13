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

namespace Owin.Webdav.Models
{
    public abstract class Resource : IResource
    {
        public Resource(IOwinContext context, string logicalPath)
        {
            OriginalContext = context;
            LogicalPath = logicalPath.Replace("\\", "/");
            _properties = new List<IProperty>();
            _properties.Add(new NumberProperty(WebdavConsts.Xml.PropGetContentLength));
            _properties.Add(new DateProperty(WebdavConsts.Xml.PropCreationDate)
            {
                Formatter = (value) => XmlConvert.ToString(value, XmlDateTimeSerializationMode.Utc) // valid rfc 3339?
            });
            _properties.Add(new DateProperty(WebdavConsts.Xml.PropGetLastModified)
            {
                FormatString = "r" // RFC1123 
            });
            _properties.Add(new DerivedProperty<string>(WebdavConsts.Xml.PropGetContentType)
            {
                 DeriveRoutine = () =>
                 {
                     return (Type == ResourceType.File) ? MimeTypes.MimeTypeMap.GetMimeType(Path.GetExtension(Name)) : null;
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
            return FindProperty<T>(name, WebdavConsts.Xml.Namespace);
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

        string _name;
        public string Name
        {
            get
            {
                return _name ?? (_name = Path.GetFileName(LogicalPath));
            }
        }
        public DerivedProperty<string> ContentType { get { return FindProperty<DerivedProperty<string>>(WebdavConsts.Xml.PropGetContentType); } }
        public DateProperty CreateDate { get { return FindProperty<DateProperty>(WebdavConsts.Xml.PropCreationDate); } }
        public DateProperty ModifyDate { get { return FindProperty<DateProperty>(WebdavConsts.Xml.PropGetLastModified); } }
        public NumberProperty Length { get { return FindProperty<NumberProperty>(WebdavConsts.Xml.PropGetContentLength); } }

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
