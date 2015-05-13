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
        public Property(string name) : this(name, WebdavConsts.Xml.Namespace)
        {

        }

        public Property(string name, string @namespace)
        {
            if (string.IsNullOrWhiteSpace(name)) { throw new ArgumentException("Name is required.", "name"); }

            Name = name;
            this.Namespace = @namespace;
        }

        public string Namespace { get; private set; }

        public string Name { get; private set; }

        public T Value { get; set; }

        public abstract XmlNode Serialize(XmlDocument doc);
    }

    public interface IProperty
    {
        string Namespace { get; }

        string Name { get; }

        XmlNode Serialize(XmlDocument doc);
    }
}
