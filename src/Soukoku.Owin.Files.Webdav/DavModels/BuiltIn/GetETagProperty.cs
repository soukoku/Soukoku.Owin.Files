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
    sealed class GetETagProperty : PropertyBase
    {
        public GetETagProperty(Resource resource) : base(resource) { }
        public override string Name
        {
            get
            {
                return DavConsts.PropertyNames.GetETag;
            }
        }

        public override void SerializeValue(XmlElement element, NewElementFunc newElementMethod)
        {
            if (element == null) { throw new ArgumentNullException("element"); }
            var value = Resource.ETag;
            if (!string.IsNullOrEmpty(value))
            {
                element.InnerText = value;
            }
        }
    }
}
