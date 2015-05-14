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
        public StringProperty(string name) : base(name, Consts.XmlNamespace) { }
        public StringProperty(string name, string @namespace) : base(name, @namespace) { }

        public override XmlElement Serialize(XmlDocument doc)
        {
            var node = doc.CreateElement(Name, Namespace);
            if (!string.IsNullOrEmpty(Value))
            {
                node.InnerText = Value;
            }
            return node;
        }
    }

    public class ReadOnlyStringProperty : DerivedProperty<string>
    {
        public ReadOnlyStringProperty(string name) : base(name, Consts.XmlNamespace) { }
        public ReadOnlyStringProperty(string name, string @namespace) : base(name, @namespace) { }


    }
}
