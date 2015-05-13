using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Soukoku.Owin.Webdav.Models
{
    public class DateProperty : Property<DateTime>
    {
        public DateProperty(string name) : base(name, Consts.Xml.Namespace) { }
        public DateProperty(string name, string @namespace) : base(name, @namespace) { }

        public string FormatString { get; set; }
        public Func<DateTime, string> Formatter { get; set; }

        public override XmlNode Serialize(XmlDocument doc)
        {
            XmlNode node = doc.CreateElement(Name, Namespace);
            if (Formatter != null)
            {
                node.InnerText = Formatter(Value);
            }
            else if (string.IsNullOrWhiteSpace(FormatString))
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
}
