using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Soukoku.Owin.Webdav.Models
{
    // property value is computed from other properties and is readonly

    public class DerivedProperty<T> : IProperty
    {
        public DerivedProperty(string name) : this(name, Consts.Xml.Namespace) { }
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
        public Func<DerivedProperty<T>, XmlDocument, XmlElement> SerializeRoutine { get; set; }

        public T Value
        {
            get
            {
                if (DeriveRoutine == null) { return default(T); }
                return DeriveRoutine();
            }
        }

        public virtual XmlElement Serialize(XmlDocument doc)
        {
            if (SerializeRoutine == null)
            {
                var node = doc.CreateElement(Name, Namespace);
                var val = Value;
                if (val != null)
                {
                    node.InnerText = val.ToString();
                }
                return node;
            }
            return SerializeRoutine(this, doc);
        }

    }
}
