using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Owin.Webdav.Models
{
    // property value is computed from other properties and is readonly

    public class DerivedProperty<T> : IProperty
    {
        public DerivedProperty(string name) : this(name, WebdavConsts.Xml.Namespace) { }
        public DerivedProperty(string name, string @namespace)
        {
            if (string.IsNullOrWhiteSpace(name)) { throw new ArgumentException("Name is required.", "name"); }

            Name = name;
            this.Namespace = @namespace;
        }

        public string Namespace { get; private set; }

        public string Name { get; private set; }

        public bool IsReadOnly { get { return true; } }

        public Func<T> DeriveRoutine { get; set; }
        public Func<XmlDocument, XmlNode> SerializeRoutine { get; set; }

        public T Value
        {
            get
            {
                if (DeriveRoutine == null) { return default(T); }
                return DeriveRoutine();
            }
        }

        public virtual XmlNode Serialize(XmlDocument doc)
        {
            if (SerializeRoutine == null)
            {
                XmlNode node = doc.CreateElement(Name, Namespace);
                var val = Value;
                if (val != null)
                {
                    node.InnerText = val.ToString();
                }
                return node;
            }
            return SerializeRoutine(doc);
        }

    }
}
