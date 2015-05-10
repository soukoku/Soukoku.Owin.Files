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

    }
}
