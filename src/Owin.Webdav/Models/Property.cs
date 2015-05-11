using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Owin.Webdav.Models
{
    public abstract class Property<T> : IProperty
    {
        public string Namespace { get; set; }

        public string Name { get; set; }

        public T Value { get; set; }

        public abstract XmlNode SerializeElement(XmlDocument doc);
    }

    public interface IProperty
    {
        string Namespace { get; set; }

        string Name { get; set; }

        XmlNode SerializeElement(XmlDocument doc);
    }
}
