using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Owin.Webdav.Responses.Properties
{
    public class ResourceType : PropertyBase
    {
        public ResourceType()
        {

        }
        public ResourceType(KnownType value)
        {
            Values = new List<KnownType> { value };
        }

        [XmlElement("collection", typeof(CollectionType))]
        public List<KnownType> Values { get; set; }

        [XmlInclude(typeof(CollectionType))]
        public abstract class KnownType { }
        
        public class CollectionType : KnownType { }
    }
}
