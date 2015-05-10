using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Owin.Webdav.Responses.Properties
{
    public class ContentType : PropertyBase
    {
        public ContentType()
        {

        }
        public ContentType(string value)
        {
            Value = value;
        }

        [XmlText]
        public string Value { get; set; }
    }
}
