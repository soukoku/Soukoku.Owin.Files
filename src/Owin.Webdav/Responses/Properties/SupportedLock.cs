using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Owin.Webdav.Responses.Properties
{
    public class SupportedLock : PropertyBase
    {
        [XmlElement("lockentry")]
        public List<LockEntry> Locks { get; set; }
    }

    public class LockEntry
    {
        [XmlArray("lockscope")]
        [XmlArrayItem("exclusive", typeof(LockScope.Exclusive))]
        [XmlArrayItem("shared", typeof(LockScope.Shared))]
        public List<LockScope> Scopes { get; set; }
        
        [XmlArray("locktype")]
        [XmlArrayItem("write", typeof(LockType.Write))]
        public List<LockType> Types { get; set; }
    }

    [XmlInclude(typeof(Exclusive))]
    [XmlInclude(typeof(Shared))]
    public abstract class LockScope
    {
        public class Exclusive : LockScope { }
        public class Shared : LockScope { }

    }


    [XmlInclude(typeof(Write))]
    public abstract class LockType
    {

        public class Write : LockType { }

    }

}
