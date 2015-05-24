using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;

namespace Soukoku.Owin.Webdav.Models.BuiltIn
{
    abstract class PropertyBase : IProperty
    {
        public PropertyBase(IResource resource)
        {
            if (resource == null) { throw new ArgumentNullException("resource"); }
            Resource = resource;
        }

        protected IResource Resource { get; private set; }

        public abstract string Name { get; }

        public string XmlNamespace { get { return DavConsts.XmlNamespace; } }

        public virtual bool IsLive { get { return true; } }

        public abstract void SerializeValue(XmlElement element, NewElementFunc newElementMethod);
        public virtual void DeserializeValue(XmlElement element) { }
    }
}
