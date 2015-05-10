using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Owin.Webdav.Responses.Properties
{
    public class CreationDate : PropertyBase
    {
        public CreationDate()
        {

        }
        public CreationDate(DateTime value)
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
                return XmlConvert.ToString(Value, XmlDateTimeSerializationMode.Utc); // rfc 3339?
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
