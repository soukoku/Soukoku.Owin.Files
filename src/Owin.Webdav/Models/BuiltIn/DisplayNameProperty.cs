using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace Soukoku.Owin.Webdav.Models.BuiltIn
{
    sealed class DisplayNameProperty : BuiltInPropertyBase
    {
        public DisplayNameProperty(IResource resource) : base(resource) { }
        public override string Name
        {
            get
            {
                return Consts.PropertyName.DisplayName;
            }
        }

        public override void SerializeValue(XPathNavigator element)
        {
            if (element == null) { throw new ArgumentNullException("element"); }
            var value = Resource.DisplayName;
            if (!string.IsNullOrEmpty(value))
            {
                value = Uri.EscapeUriString(value);
                if (!string.IsNullOrEmpty(value))
                {
                    element.SetValue(value);
                }
            }
        }
    }
}
