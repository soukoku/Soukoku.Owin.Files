using Soukoku.Owin.Files.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;

namespace Soukoku.Owin.Files.DavModels.BuiltIn
{
    sealed class CreationDateProperty : PropertyBase
    {
        public CreationDateProperty(Resource resource) : base(resource) { }
        public override string Name
        {
            get
            {
                return DavConsts.PropertyNames.CreationDate;
            }
        }

        public override void SerializeValue(XmlElement element, NewElementFunc newElementMethod)
        {
            if (element == null) { throw new ArgumentNullException("element"); }
            var value = Resource.CreationDateUtc;
            if (value > DateTime.MinValue)
            {
                element.InnerText = XmlConvert.ToString(value, XmlDateTimeSerializationMode.Utc); // valid rfc 3339?
            }
        }
    }
}
