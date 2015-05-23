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
    sealed class ResourceTypeProperty : PropertyBase
    {
        public ResourceTypeProperty(IResource resource) : base(resource) { }
        public override string Name
        {
            get
            {
                return DavConsts.PropertyNames.ResourceType;
            }
        }

        public override void SerializeValue(XPathNavigator element)
        {
            if (element == null) { throw new ArgumentNullException("element"); }
            if (Resource.ResourceType == ResourceType.Collection)
            {
                var pfx = element.LookupPrefix(DavConsts.XmlNamespace);
                element.AppendChildElement(pfx, DavConsts.ElementNames.Collection, DavConsts.XmlNamespace, null);
            }
        }
    }
}
