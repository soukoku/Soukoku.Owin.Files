using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Owin.Webdav.Responses.Properties
{
    public class ModifiedDate : PropertyBase
    {
        public ModifiedDate()
        {

        }
        public ModifiedDate(DateTime value)
        {
            Value = value;
        }


        [XmlIgnore]
        public DateTime Value { get; set; }

        [XmlText, EditorBrowsable(EditorBrowsableState.Never)]
        public string SerializedValue
        {
            get
            {
                return Value.ToString("r"); // RFC1123
            }
            set
            {
                DateTime test;
                if (DateTime.TryParse(value, out test))
                {
                    Value = test;
                }
            }
        }

    }
}
