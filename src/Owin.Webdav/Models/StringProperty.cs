using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Soukoku.Owin.Webdav.Models
{
    public class StringProperty : Property<string>
    {
        public StringProperty(string name) : base(name, Consts.Xml.Namespace) { }
        public StringProperty(string name, string @namespace) : base(name, @namespace) { }

        public override XmlNode Serialize(XmlDocument doc)
        {
            XmlNode node = doc.CreateElement(Name, Namespace);
            if (!string.IsNullOrEmpty(Value))
            {
                node.InnerText = Value;
            }
            return node;
        }
    }
}
