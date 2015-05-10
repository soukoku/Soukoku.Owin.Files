using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Owin.Webdav.Models
{
    public abstract class Resource
    {
        public Resource(string logicalPath)
        {
            LogicalPath = logicalPath ?? string.Empty;
        }
        public string LogicalPath { get; private set; }

        string _name;
        public string Name
        {
            get
            {
                return _name ?? (_name = Path.GetFileName(LogicalPath));
            }
        }

        public virtual DateTime CreateDate { get { return DateTime.MinValue; } }
        public virtual DateTime ModifyDate { get { return DateTime.MinValue; } }
        public virtual long Length { get { return 0; } }
        public abstract ResourceType Type { get; }

        public override string ToString()
        {
            return LogicalPath;
        }

        public virtual Stream GetReadStream()
        {
            throw new NotSupportedException();
        }
    }
}
