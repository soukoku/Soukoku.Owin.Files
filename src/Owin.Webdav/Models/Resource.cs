using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Owin.Webdav.Models
{
    public abstract class Resource
    {
        public Resource(IOwinContext context, string logicalPath)
        {
            OriginalContext = context;
            LogicalPath = logicalPath;
            CustomProperties = new Dictionary<string, IProperty>(StringComparer.OrdinalIgnoreCase);
        }

        public Dictionary<string, IProperty> CustomProperties { get; private set; }


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
        public virtual string ContentType
        {
            get
            {
                if (Type == ResourceType.File)
                {
                    return MimeTypes.MimeTypeMap.GetMimeType(Path.GetExtension(Name));
                }
                return null;
            }
        }
        public virtual DateTime CreateDate { get { return DateTime.MinValue; } }
        public virtual DateTime ModifyDate { get { return DateTime.MinValue; } }
        public virtual long Length { get { return 0; } }
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
