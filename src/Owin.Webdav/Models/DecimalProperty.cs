using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Soukoku.Owin.Webdav.Models
{
    public class DecimalProperty : Property<decimal>
    {
        public DecimalProperty(string name) : base(name, Consts.XmlNamespace) { }
        public DecimalProperty(string name, string @namespace) : base(name, @namespace) { }

        public string FormatString { get; set; }

        public override XmlElement Serialize(XmlDocument doc)
        {
            var node = doc.CreateElement(Name, Namespace);
            if (string.IsNullOrWhiteSpace(FormatString))
            {
                node.InnerText = Value.ToString();
            }
            else
            {
                node.InnerText = Value.ToString(FormatString);
            }
            return node;
        }
    }

    public class ReadOnlyDecimalProperty : DerivedProperty<decimal>
    {
        public ReadOnlyDecimalProperty(string name) : base(name, Consts.XmlNamespace) { }
        public ReadOnlyDecimalProperty(string name, string @namespace) : base(name, @namespace) { }

        public string FormatString { get; set; }
        public Func<DateTime, string> Formatter { get; set; }


        public override XmlElement Serialize(XmlDocument doc)
        {
            if (SerializeRoutine == null)
            {
                var node = doc.CreateElement(Name, Namespace);
                if (string.IsNullOrWhiteSpace(FormatString))
                {
                    node.InnerText = Value.ToString();
                }
                else
                {
                    node.InnerText = Value.ToString(FormatString);
                }
                return node;
            }
            return SerializeRoutine(this, doc);
        }
    }
}
