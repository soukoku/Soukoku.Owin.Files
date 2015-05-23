using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;

namespace Soukoku.Owin.Webdav.Models.BuiltIn
{
    sealed class ResourceTypeProperty : BuiltInPropertyBase
    {
        public ResourceTypeProperty(IResource resource) : base(resource) { }
        public override string Name
        {
            get
            {
                return Consts.PropertyName.ResourceType;
            }
        }

        public override void SerializeValue(XPathNavigator element)
        {
            if (element == null) { throw new ArgumentNullException("element"); }
            if (Resource.Type == ResourceType.Collection)
            {
                var pfx = element.LookupPrefix(Consts.XmlNamespace);
                element.AppendChildElement(pfx, Consts.ElementName.Collection, Consts.XmlNamespace, null);
            }
        }
    }
}
