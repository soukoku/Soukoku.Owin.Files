using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Owin.Webdav.Responses
{
    public class ResourceResponse
    {
        [XmlElement("href")]
        public string Href { get; set; }

        [XmlElement("propstat")]
        public PropertyStat ProperyStat { get; set; }
    }
}
