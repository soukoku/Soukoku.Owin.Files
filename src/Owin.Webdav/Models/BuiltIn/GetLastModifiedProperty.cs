using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace Soukoku.Owin.Webdav.Models.BuiltIn
{
    sealed class GetLastModifiedProperty : PropertyBase
    {
        public GetLastModifiedProperty(IResource resource) : base(resource) { }
        public override string Name
        {
            get
            {
                return Consts.PropertyName.GetLastModified;
            }
        }

        public override void SerializeValue(XPathNavigator element)
        {
            if (element == null) { throw new ArgumentNullException("element"); }
            var value = Resource.ModifiedDateUtc;
            if (value > DateTime.MinValue)
                element.SetValue(value.ToString("r", CultureInfo.InvariantCulture)); // RFC1123
        }
    }
}
