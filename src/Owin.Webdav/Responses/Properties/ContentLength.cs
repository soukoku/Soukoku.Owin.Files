using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Owin.Webdav.Responses.Properties
{
    public class ContentLength : PropertyBase
    {
        public ContentLength()
        {

        }
        public ContentLength(long value)
        {
            Value = value;
        }

        [XmlIgnore]
        public long Value { get; set; }

        [XmlText, EditorBrowsable(EditorBrowsableState.Never)]
        public string SerializedValue
        {
            get
            {
                return Value.ToString();
            }
            set
            {
                long test;
                if (long.TryParse(value, out test))
                {
                    Value = test;
                }
            }
        }
    }
}
