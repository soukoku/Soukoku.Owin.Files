using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace Soukoku.Owin.Webdav.Models.BuiltIn
{
    sealed class GetContentLanguageProperty : PropertyBase
    {
        public GetContentLanguageProperty(IResource resource) : base(resource) { }
        public override string Name
        {
            get
            {
                return Consts.PropertyName.GetContentLanguage;
            }
        }

        public override void SerializeValue(XPathNavigator element)
        {
            if (element == null) { throw new ArgumentNullException("element"); }
            var value = Resource.ContentLanguage;
            if (!string.IsNullOrEmpty(value))
                element.SetValue(value);
        }
    }
}
