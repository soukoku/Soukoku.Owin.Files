using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace Soukoku.Owin.Webdav.Models.BuiltIn
{
    abstract class PropertyBase : IDavProperty
    {
        public PropertyBase(IResource resource)
        {
            if (resource == null) { throw new ArgumentNullException("resource"); }
            Resource = resource;
        }

        protected IResource Resource { get; private set; }

        public abstract string Name { get; }

        public string NamespaceUri { get { return Consts.XmlNamespace; } }

        public abstract void SerializeValue(XPathNavigator element);
    }
}
