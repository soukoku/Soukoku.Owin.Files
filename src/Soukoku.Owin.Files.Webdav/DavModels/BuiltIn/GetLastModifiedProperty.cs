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
    sealed class GetLastModifiedProperty : PropertyBase
    {
        public GetLastModifiedProperty(Resource resource) : base(resource) { }
        public override string Name
        {
            get
            {
                return DavConsts.PropertyNames.GetLastModified;
            }
        }

        public override void SerializeValue(XmlElement element, NewElementFunc newElementMethod)
        {
            if (element == null) { throw new ArgumentNullException("element"); }
            var value = Resource.ModifiedDateUtc;
            if (value > DateTime.MinValue)
                element.InnerText = value.ToString("r", CultureInfo.InvariantCulture); // RFC1123
        }
    }
}
