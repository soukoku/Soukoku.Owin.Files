using Soukoku.Owin.Files.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;

namespace Soukoku.Owin.Files.Webdav.DavModels.BuiltIn
{
    sealed class ResourceTypeProperty : PropertyBase
    {
        public ResourceTypeProperty(Resource resource) : base(resource) { }
        public override string Name
        {
            get
            {
                return DavConsts.PropertyNames.ResourceType;
            }
        }

        public override void SerializeValue(XmlElement element, NewElementFunc newElementMethod)
        {
            if (element == null) { throw new ArgumentNullException("element"); }
            if (newElementMethod == null) { throw new ArgumentNullException("newElementMethod"); }
            if (Resource.IsFolder)
            {
                var subEl = newElementMethod(DavConsts.ElementNames.Collection, DavConsts.XmlNamespace);
                element.AppendChild(subEl);
            }
        }
    }
}
